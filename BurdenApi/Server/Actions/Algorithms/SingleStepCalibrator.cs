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
            ? await AdjustResistance(deviation, context) ?? await AdjustImpedance(deviation, context)
            : await AdjustImpedance(deviation, context) ?? await AdjustResistance(deviation, context);
    }

    /// <summary>
    /// Calibrate the resistence pair.
    /// </summary>
    /// <param name="valueDeviation">Deviation measured.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustResistance(GoalDeviation valueDeviation, ICalibrationContext context)
        => AdjustCoarseAndFine(
            _ResistanceCoarseFixed,
            valueDeviation,
            context.CurrentCalibration.Resistive,
            context,
            deviation => deviation.DeltaPower,
            calibrationPair => new(calibrationPair, context.CurrentCalibration.Inductive),
            () => _ResistanceCoarseFixed = true
        );

    /// <summary>
    /// Calibrate the impedance pair.
    /// </summary>
    /// <param name="valueDeviation">Deviation measured.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustImpedance(GoalDeviation valueDeviation, ICalibrationContext context)
        => AdjustCoarseAndFine(
            _ImpedanceCoarseFixed,
            valueDeviation,
            context.CurrentCalibration.Inductive,
            context,
            deviation => deviation.DeltaFactor,
            calibrationPair => new(context.CurrentCalibration.Resistive, calibrationPair),
            () => _ImpedanceCoarseFixed = true
        );

    /// <summary>
    /// Calibrate a pair of values.
    /// </summary>
    /// <param name="coarseAlreadyFixed">Set if the coarse value is already fixed.</param>
    /// <param name="valueDeviation">Measured deviation from the goal.</param>
    /// <param name="calibration">Calibration pair to process.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <param name="getDeviationField">Access the deviation to inspect.</param>
    /// <param name="createCalibration">Method to create a full calibration set to get a new measurement.</param>
    /// <param name="setCoarseAlreadyFixed">Set to fix the coarse calibration.</param>
    /// <returns>null if no futher processing is possible.</returns>
    private async Task<CalibrationStep?> AdjustCoarseAndFine(
        bool coarseAlreadyFixed,
        GoalDeviation valueDeviation,
        CalibrationPair calibration,
        ICalibrationContext context,
        Func<GoalDeviation, double> getDeviationField,
        Func<CalibrationPair, Calibration> createCalibration,
        Action setCoarseAlreadyFixed
    )
    {
        // No difference at all.
        if (getDeviationField(valueDeviation) == 0) return null;

        // Adjust coarse corrextion if not already fixed.
        if (!coarseAlreadyFixed)
        {
            // Check if there is a better calibration.
            var coraseStep = await AdjustCoarseOrFine(valueDeviation, context, getDeviationField, calibration.ChangeCoarse, createCalibration);

            if (coraseStep != null)
            {
                // We found a better match.
                if (coraseStep.CalibrationChanged) return coraseStep;

                // There is no better match in coarse, so fix the calibration value and try the fine value.
                setCoarseAlreadyFixed();
            }
        }

        // Adjust fine correction.
        var fineStep = await AdjustCoarseOrFine(valueDeviation, context, getDeviationField, calibration.ChangeFine, createCalibration);

        // If fine correction step makes it worse just skip it.
        return fineStep?.CalibrationChanged == true ? fineStep : null;
    }

    /// <summary>
    /// Adjust a single part (e.g. GoalValue.ApparentPower) of a single pair (e.g. Calibration.Resistive).
    /// </summary>
    /// <param name="valueDeviation">Deviations measured.</param>
    /// <param name="context">Current calibration environment.</param>
    /// <param name="getDeviationField">Access the deviation information to use.</param>
    /// <param name="updateCalibrationValue">Update the calibration for the next step.</param>
    /// <param name="createCalibration">Create a complete calibration information for value measurement.</param>
    /// <returns>null if no further processing is possible, elsewhere the Calibration memember will be 
    /// null as well if the changed calibration moved us further away from the goal.</returns>
    private async Task<CalibrationStep?> AdjustCoarseOrFine(
        GoalDeviation valueDeviation,
        ICalibrationContext context,
        Func<GoalDeviation, double> getDeviationField,
        Func<bool, CalibrationPair?> updateCalibrationValue,
        Func<CalibrationPair, Calibration> createCalibration
    )
    {
        var deviation = getDeviationField(valueDeviation);

        // No difference at all - access part (e.g. GoalValue.ApparentPower) using accessor method field
        if (deviation == 0) return null;

        // Apply correction (true means increment, false decrement) - it is expected that the order of calibration values mirrors the order of measured values.
        var nextPair = updateCalibrationValue(deviation < 0);

        // Correction may not be possible - bounds are reached.
        if (nextPair == null) return null;

        // Re-measure - rebuild calibration using pair (e.g Calibration.Resistive) keeping the other part.
        var nextCalibration = createCalibration(nextPair);
        var nextValues = await context.MeasureAsync(nextCalibration);
        var nextDelta = nextValues / context.EffectiveGoal;

        // We made it worse or nothing changed at all - but indicated with Calibration null that we at least gave it a try.
        if (Math.Abs(getDeviationField(nextDelta)) >= Math.Abs(deviation)) return new() { Calibration = null! };

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