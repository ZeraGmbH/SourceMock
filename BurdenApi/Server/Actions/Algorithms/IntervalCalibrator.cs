using BurdenApi.Models;
using ZstdSharp.Unsafe;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Calibration algorithm single stepping values.
/// </summary>
public class IntervalCalibrator : ICalibrationAlgorithm
{
    /// <summary>
    /// Steps of operation.
    /// </summary>
    private enum Phases
    {
        RestistanceCoarse = 0,

        ResistanceFine = 1,

        ImpedanceCoarse = 2,

        ImpedanceFine = 3,

        Adjust = 4,
    }

    /// <summary>
    /// Current phase.
    /// </summary>
    private Phases _Phase = Phases.RestistanceCoarse;

    /// <summary>
    /// Current interval.
    /// </summary>
    private int _Width = 64;

    /// <inheritdoc/>
    public Calibration CreateInitialCalibration(Calibration calibration) => new(new(64, 0), new(0, 0));

    private bool ResistancePhase => _Phase == Phases.RestistanceCoarse || _Phase == Phases.ResistanceFine;

    /// <inheritdoc/>
    public async Task<CalibrationStep?> IterateAsync(ICalibrationContext context)
    {
        // Interval phase.
        while (_Phase < Phases.Adjust)
        {
            // Check if we are in interval mode - will stop on an exact match whcih is really unexpected but still possible.        
            if (_Width >= 0)
            {
                // Interval will not reach calibration value 0 without a little tweak.
                var deviation = ResistancePhase ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor;
                var alreadyDone = _Width == 0;

                // Regular interval step.
                _Width = alreadyDone ? -1 : _Width / 2;

                // Calculate difference.
                var delta = alreadyDone ? 1 : deviation > 0 ? -_Width : +_Width;
                var appliedDelta = ResistancePhase ? +delta : -delta;

                if (!alreadyDone || (GetActiveCalibration(context) == 1 && appliedDelta == -1))
                {
                    // If measure value is larger than expected value step back a bit.
                    var nextStep = await IntervalStep(context, appliedDelta);

                    if (nextStep != null) return nextStep;
                }
            }

            // Reset interval.
            _Width = 64;

            // Next phase.
            _Phase++;

            // Measure the initial calibration.
            var initialStep = await MeasureAsync(context,
                _Phase switch
                {
                    Phases.ResistanceFine => new CalibrationPair(context.LastStep.Calibration.Resistive.Coarse, 64),
                    Phases.ImpedanceCoarse => new CalibrationPair(64, 0),
                    Phases.ImpedanceFine => new CalibrationPair(context.LastStep.Calibration.Inductive.Coarse, 64),
                    _ => null
                });

            if (initialStep != null) return initialStep;
        }

        return null;
    }

    /// <summary>
    /// Get current values.
    /// </summary>
    /// <param name="context">Optation context.</param>
    /// <param name="nextPair">Calibration pair to apply.</param>
    /// <returns>Measured values.</returns>
    private async Task<CalibrationStep?> MeasureAsync(ICalibrationContext context, CalibrationPair? nextPair)
    {
        // Nothing to do.
        if (nextPair == null) return null;

        // Re-measure.
        var calibration = context.CurrentCalibration;
        var nextCalibration = new Calibration(ResistancePhase ? nextPair : calibration.Resistive, ResistancePhase ? calibration.Inductive : nextPair);
        var nextValues = await context.MeasureAsync(nextCalibration);
        var nextDelta = nextValues / context.EffectiveGoal;

        // Apply measurement values from the burden as well.
        var burdenValues = await context.MeasureBurdenAsync();
        var values = new GoalValue { ApparentPower = burdenValues.ApparentPower, PowerFactor = burdenValues.PowerFactor };

        return new()
        {
            BurdenDeviation = values / context.EffectiveGoal,
            BurdenValues = values,
            Calibration = nextCalibration,
            Deviation = nextDelta,
            Values = nextValues,
        };
    }

    /// <summary>
    /// Get the current calibration value.
    /// </summary>
    /// <param name="context">Operation context.</param>
    /// <returns>Calibration value or null if not in interval phase.</returns>
    private byte? GetActiveCalibration(ICalibrationContext context)
        => _Phase switch
        {
            Phases.RestistanceCoarse => context.LastStep.Calibration.Resistive.Coarse,
            Phases.ResistanceFine => context.LastStep.Calibration.Resistive.Fine,
            Phases.ImpedanceCoarse => context.LastStep.Calibration.Inductive.Coarse,
            Phases.ImpedanceFine => context.LastStep.Calibration.Inductive.Fine,
            _ => null
        };

    /// <summary>
    /// Still in the interval phase.
    /// </summary>
    /// <param name="context">Processing context including the current state.</param>
    /// <param name="delta">Calibratioin change to apply.</param>
    /// <returns>The result of the measurement.</returns>
    private async Task<CalibrationStep?> IntervalStep(ICalibrationContext context, int delta)
        => await MeasureAsync(context,
            _Phase switch
            {
                Phases.RestistanceCoarse => context.LastStep.Calibration.Resistive.ChangeCoarse(delta),
                Phases.ResistanceFine => context.LastStep.Calibration.Resistive.ChangeFine(delta),
                Phases.ImpedanceCoarse => context.LastStep.Calibration.Inductive.ChangeCoarse(delta),
                Phases.ImpedanceFine => context.LastStep.Calibration.Inductive.ChangeFine(delta),
                _ => null
            });

    /// <inheritdoc/>
    public bool ContinueAfterCycleDetection()
    {
        return true;
    }
}