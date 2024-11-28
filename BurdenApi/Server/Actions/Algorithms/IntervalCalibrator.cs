using BurdenApi.Models;

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
                var partDeviation = ResistancePhase ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor;
                var alreadyDone = _Width == 0;

                // Regular interval step.
                _Width = alreadyDone ? -1 : _Width / 2;

                // Calculate difference.
                var delta = alreadyDone ? 1 : partDeviation >= 0 ? -_Width : +_Width;
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
            var initialStep = await MeasureAsync(ResistancePhase, context,
                _Phase switch
                {
                    Phases.ResistanceFine => new CalibrationPair(context.LastStep.Calibration.Resistive.Coarse, 64),
                    Phases.ImpedanceCoarse => new CalibrationPair(64, 0),
                    Phases.ImpedanceFine => new CalibrationPair(context.LastStep.Calibration.Inductive.Coarse, 64),
                    _ => null
                });

            if (initialStep != null) return initialStep;

            // Will no enter adjustment mode.
            context.ClearCycleTester();
        }

        // Retrieve new values and relativ deviation from goal.
        var deviation = context.LastStep.Deviation;

        // Work on the largest deviation first.
        return
            Math.Abs(deviation.DeltaPower) >= Math.Abs(deviation.DeltaFactor)
            ? await AdjustResistance(context) ?? await AdjustImpedance(context)
            : await AdjustImpedance(context) ?? await AdjustResistance(context);
    }

    /// <summary>
    /// Calibrate the resistance pair.
    /// </summary>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustResistance(ICalibrationContext context) => AdjustFine(true, context);

    /// <summary>
    /// Calibrate the impedance pair.
    /// </summary>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustImpedance(ICalibrationContext context) => AdjustFine(false, context);

    /// <summary>
    /// Get current values.
    /// </summary>
    /// <param name="resistiveNotImpedance">Set if working on resistive pair.</param>
    /// <param name="context">Optation context.</param>
    /// <param name="nextPair">Calibration pair to apply.</param>
    /// <param name="isAcceptable">Optional test if the new values are acceptabble.</param>
    /// <returns>Measured values.</returns>
    private async Task<CalibrationStep?> MeasureAsync(bool resistiveNotImpedance, ICalibrationContext context, CalibrationPair? nextPair, Func<GoalDeviation, bool>? isAcceptable = null)
    {
        // Nothing to do.
        if (nextPair == null) return null;

        // Re-measure.
        var calibration = context.CurrentCalibration;
        var nextCalibration = new Calibration(resistiveNotImpedance ? nextPair : calibration.Resistive, resistiveNotImpedance ? calibration.Inductive : nextPair);
        var nextValues = await context.MeasureAsync(nextCalibration);
        var nextDelta = nextValues / context.EffectiveGoal;

        // See if we made it better.
        if (isAcceptable != null && !isAcceptable(nextDelta)) return null;

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
        => await MeasureAsync(ResistancePhase, context,
            _Phase switch
            {
                Phases.RestistanceCoarse => context.LastStep.Calibration.Resistive.ChangeCoarse(delta),
                Phases.ResistanceFine => context.LastStep.Calibration.Resistive.ChangeFine(delta),
                Phases.ImpedanceCoarse => context.LastStep.Calibration.Inductive.ChangeCoarse(delta),
                Phases.ImpedanceFine => context.LastStep.Calibration.Inductive.ChangeFine(delta),
                _ => null
            });

    /// <inheritdoc/>
    public bool ContinueAfterCycleDetection() => false;

    /// <summary>
    /// Calibrate a pair of values.
    /// </summary>
    /// <param name="resistiveNotImpedance">Set if working on resistive pair.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>null if no futher processing is possible.</returns>
    private Task<CalibrationStep?> AdjustFine(bool resistiveNotImpedance, ICalibrationContext context)
    {
        var deviation = resistiveNotImpedance ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor;

        // No difference at all - access part (e.g. GoalValue.ApparentPower) using accessor method field
        if (deviation == 0) return Task.FromResult<CalibrationStep?>(null);

        // What part are we working of.
        var calibration = resistiveNotImpedance ? context.CurrentCalibration.Resistive : context.CurrentCalibration.Inductive;

        // Try to adjust fine value.
        var delta = (deviation < 0) == resistiveNotImpedance ? +1 : -1;
        var nextPair = calibration.ChangeFine(delta);

        if (nextPair == null)
        {
            // Try to adjust coarse value.
            nextPair = calibration.ChangeCoarse(delta);

            // Set fine value to center.
            if (nextPair != null) nextPair = new(nextPair.Coarse, 64);
        }

        // Measure but only accept the new value if we made it some better.
        return MeasureAsync(
            resistiveNotImpedance,
            context,
            nextPair,
            nextDelta => Math.Abs(resistiveNotImpedance ? nextDelta.DeltaPower : nextDelta.DeltaFactor) < Math.Abs(deviation)
        );
    }
}