using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Calibration algorithm single stepping values.
/// </summary>
public class SingleStepCalibrator : ICalibrationAlgorithm
{
    private bool _ResistanceCoarseFixed = false;

    private bool _ImpedanceCoarseFixed = false;

    /// <inheritdoc/>
    public Calibration CreateInitialCalibration(Calibration calibration)
        => new(new(calibration.Resistive.Coarse, 64), new(calibration.Inductive.Coarse, 64));

    /// <inheritdoc/>
    public async Task<CalibrationStep?> IterateAsync(ICalibrationContext context)
    {
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
    private Task<CalibrationStep?> AdjustResistance(ICalibrationContext context) => AdjustCoarseAndFine(true, context);

    /// <summary>
    /// Calibrate the impedance pair.
    /// </summary>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustImpedance(ICalibrationContext context) => AdjustCoarseAndFine(false, context);

    /// <summary>
    /// Calibrate a pair of values.
    /// </summary>
    /// <param name="resistiveNotImpedance">Set if working on resistive pair.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>null if no futher processing is possible.</returns>
    private async Task<CalibrationStep?> AdjustCoarseAndFine(
        bool resistiveNotImpedance,
        ICalibrationContext context
    )
    {
        // No difference at all.
        if ((resistiveNotImpedance ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor) == 0) return null;

        // What part are we working of.
        var calibration = resistiveNotImpedance ? context.CurrentCalibration.Resistive : context.CurrentCalibration.Inductive;

        // Adjust coarse corrextion if not already fixed.
        if (!(resistiveNotImpedance ? _ResistanceCoarseFixed : _ImpedanceCoarseFixed))
        {
            // Check if there is a better calibration.
            var coraseStep = await AdjustCoarseOrFine(resistiveNotImpedance, context, calibration.ChangeCoarse);

            if (coraseStep != null)
            {
                // We found a better match.
                if (coraseStep.CalibrationChanged) return coraseStep;

                // There is no better match in coarse, so fix the calibration value and try the fine value.
                if (resistiveNotImpedance) _ResistanceCoarseFixed = true; else _ImpedanceCoarseFixed = true;
            }
        }

        // Adjust fine correction.
        var fineStep = await AdjustCoarseOrFine(resistiveNotImpedance, context, calibration.ChangeFine);

        // If fine correction step makes it worse just skip it.
        return fineStep?.CalibrationChanged == true ? fineStep : null;
    }

    /// <summary>
    /// Adjust a single part (e.g. GoalValue.ApparentPower) of a single pair (e.g. Calibration.Resistive).
    /// </summary>
    /// <param name="resistiveNotImpedance">Set if working on resistive pair.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <param name="updateCalibrationValue">Update the calibration for the next step.</param>
    /// <returns>null if no further processing is possible, elsewhere the Calibration memember will be 
    /// null as well if the changed calibration moved us further away from the goal.</returns>
    private async Task<CalibrationStep?> AdjustCoarseOrFine(
        bool resistiveNotImpedance,
        ICalibrationContext context,
        Func<int, bool, CalibrationPair?> updateCalibrationValue
    )
    {
        var deviation = resistiveNotImpedance ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor;

        // No difference at all - access part (e.g. GoalValue.ApparentPower) using accessor method field
        if (deviation == 0) return null;

        // Apply correction (true means increment, false decrement, impedance is 1 at calibration 0) - it is expected that the order of calibration values mirrors the order of measured values.
        var nextPair = updateCalibrationValue(((deviation < 0) == resistiveNotImpedance) ? +1 : -1, false);

        // Correction may not be possible - bounds are reached.
        if (nextPair == null) return null;

        // Re-measure - rebuild calibration using pair (e.g Calibration.Resistive) keeping the other part.
        var nextCalibration = resistiveNotImpedance
            ? new Calibration(nextPair, context.CurrentCalibration.Inductive)
            : new Calibration(context.CurrentCalibration.Resistive, nextPair);

        var nextValues = await context.MeasureAsync(nextCalibration);
        var nextDelta = context.MakeDeviation(nextValues, nextValues.Rms);

        // We made it worse or nothing changed at all - but indicated with Calibration null that we at least gave it a try.
        if (Math.Abs(resistiveNotImpedance ? nextDelta.DeltaPower : nextDelta.DeltaFactor) >= Math.Abs(deviation))
            return new() { Calibration = null!, Deviation = nextDelta, Values = nextValues };

        // Apply measurement values from the burden as well.
        var burdenValues = await context.MeasureBurdenAsync();

        return new()
        {
            BurdenDeviation = context.MakeDeviation(burdenValues, nextValues.Rms),
            BurdenValues = burdenValues,
            Calibration = nextCalibration,
            Deviation = nextDelta,
            Values = nextValues,
        };
    }

    /// <inheritdoc/>
    public bool ContinueAfterCycleDetection()
    {
        // No repeat allowed if coarse values are fixed - assume fine changes not good enough.
        if (_ImpedanceCoarseFixed && _ResistanceCoarseFixed) return false;

        // Lock out - will not try again.
        _ImpedanceCoarseFixed = true;
        _ResistanceCoarseFixed = true;

        // Try again.
        return true;
    }
}