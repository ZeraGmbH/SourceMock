using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Calibration algorithm single stepping values.
/// </summary>
public class FineFirstCalibrator : ICalibrationAlgorithm
{
    /// <inheritdoc/>
    public bool ContinueAfterCycleDetection() => false;

    /// <inheritdoc/>
    public Calibration CreateInitialCalibration(Calibration calibration) => calibration;

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

        // Try fine steps.
        if ((calibration.Coarse == 0 || calibration.Fine >= 10) && (calibration.Coarse == 127 || calibration.Fine <= 120))
            for (int step = 20; step > 0; step /= 2)
            {
                // Move forward.
                var fineStep = await AdjustCoarseOrFine(resistiveNotImpedance, context, calibration.ChangeFine, step);

                // Not possible at all - we reached a bound.
                if (fineStep?.Calibration != null) return fineStep;
            }

        // Try coarse.
        var coarseStep = await AdjustCoarseOrFine(resistiveNotImpedance, context, (int delta, bool clip) =>
        {
            // Try to move coarse.
            var next = calibration.ChangeCoarse(delta, clip);

            // If successfull center fine.
            return (next == null) ? null : new CalibrationPair(next.Coarse, 64);
        }, 1);

        return coarseStep?.Calibration == null ? null : coarseStep;
    }

    /// <summary>
    /// Adjust a single part (e.g. GoalValue.ApparentPower) of a single pair (e.g. Calibration.Resistive).
    /// </summary>
    /// <param name="resistiveNotImpedance">Set if working on resistive pair.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <param name="updateCalibrationValue">Update the calibration for the next step.</param>
    /// <param name="step">Step to take.</param>
    /// <returns>null if no further processing is possible, elsewhere the Calibration memember will be 
    /// null as well if the changed calibration moved us further away from the goal.</returns>
    private async Task<CalibrationStep?> AdjustCoarseOrFine(
        bool resistiveNotImpedance,
        ICalibrationContext context,
        Func<int, bool, CalibrationPair?> updateCalibrationValue,
        int step
    )
    {
        var deviation = resistiveNotImpedance ? context.LastStep.Deviation.DeltaPower : context.LastStep.Deviation.DeltaFactor;

        // No difference at all - access part (e.g. GoalValue.ApparentPower) using accessor method field
        if (deviation == 0) return null;

        // Apply correction (true means increment, false decrement, impedance is 1 at calibration 0) - it is expected that the order of calibration values mirrors the order of measured values.
        var nextPair = updateCalibrationValue(((deviation < 0) == resistiveNotImpedance) ? +step : -step, true);

        // Correction may not be possible - bounds are reached.
        if (nextPair == null) return null;

        // Re-measure - rebuild calibration using pair (e.g Calibration.Resistive) keeping the other part.
        var nextCalibration = resistiveNotImpedance
            ? new Calibration(nextPair, context.CurrentCalibration.Inductive)
            : new Calibration(context.CurrentCalibration.Resistive, nextPair);

        var nextValues = await context.MeasureAsync(nextCalibration);
        var nextDelta = context.MakeDeviation(nextValues, nextValues.Range);

        // We made it worse or nothing changed at all - but indicated with Calibration null that we at least gave it a try.
        if (Math.Abs(resistiveNotImpedance ? nextDelta.DeltaPower : nextDelta.DeltaFactor) >= Math.Abs(deviation))
            return new() { Calibration = null!, Deviation = nextDelta, Values = nextValues };

        // Apply measurement values from the burden as well.
        var burdenValues = await context.MeasureBurdenAsync();

        return new()
        {
            BurdenDeviation = context.MakeDeviation(burdenValues, nextValues.Range),
            BurdenValues = burdenValues,
            Calibration = nextCalibration,
            Deviation = nextDelta,
            Values = nextValues,
        };
    }
}