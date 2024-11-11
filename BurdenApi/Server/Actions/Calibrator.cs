using BurdenApi.Models;

namespace BurdenApi.Actions;

/// <summary>
/// Implements one burden calibration algorithm.
/// </summary>
public class Calibrator(ICalibrationHardware hardware) : ICalibrator
{
    /// <inheritdoc/>
    public Calibration InitialCalibration { get; private set; } = null!;

    /// <inheritdoc/>
    public GoalValue Goal { get; private set; } = null!;

    private Calibration CurrentCalibration => LastStep?.Calibration ?? InitialCalibration;

    private bool _ResistanceCoarseFixed = false;

    private bool _ImpedanceCoarseFixed = false;

    private readonly List<CalibrationStep> _Steps = [];

    /// <inheritdoc/>
    public CalibrationStep[] Steps => [.. _Steps];

    /// <inheritdoc/>
    public CalibrationStep? LastStep => _Steps.LastOrDefault();

    /// <inheritdoc/>
    public async Task RunAsync(CalibrationRequest request)
    {
        Goal = request.Goal;
        InitialCalibration = request.InitialCalibration;

        // Reset state - caller is responsible to synchronize access.
        _ImpedanceCoarseFixed = false;
        _ResistanceCoarseFixed = false;

        _Steps.Clear();

        // Secure bound by a maximum number of steps - we expect a maximum of 4 * 127 steps.
        for (var done = new HashSet<CalibrationStep>(); _Steps.Count < 1000;)
        {
            // Try to make it better.
            var step = await IterateAsync();

            // We found no way to improve the calibration.
            if (step == null) break;

            // Already did this one?
            if (!done.Add(step))
            {
                // Already fixed anything - must abort now.
                if (_ImpedanceCoarseFixed && _ResistanceCoarseFixed) break;

                // Add to list.
                _Steps.Add(step);

                // Find the best fit - will become the latest result.
                step = _Steps.MinBy(s => s.TotalAbsDelta)!;

                // Only fine tuning for now.
                _ImpedanceCoarseFixed = true;
                _ResistanceCoarseFixed = true;
            }

            // Add to list.
            _Steps.Add(step);

            // Just to give a hint on how many work we did.
            step.Iteration = _Steps.Count;
        }
    }

    /// <summary>
    /// Execute one calibration iteration step.
    /// </summary>
    /// <returns>Successfull calibration step or null.</returns>
    private async Task<CalibrationStep?> IterateAsync()
    {
        // Retrieve new values and relativ deviation from goal.
        var deviation = await hardware.MeasureAsync(CurrentCalibration) / Goal;

        // Work on the largest deviation first.
        return
            Math.Abs(deviation.DeltaPower) >= Math.Abs(deviation.DeltaFactor)
            ? await AdjustResistance(deviation) ?? await AdjustImpedance(deviation)
            : await AdjustImpedance(deviation) ?? await AdjustResistance(deviation);
    }

    /// <summary>
    /// Calibrate the resistence pair.
    /// </summary>
    /// <param name="valueDeviation">Deviation measured.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustResistance(GoalDeviation valueDeviation)
        => AdjustCoarseAndFine(
            _ResistanceCoarseFixed,
            valueDeviation,
            CurrentCalibration.Resistive,
            deviation => deviation.DeltaPower,
            calibrationPair => new(calibrationPair, CurrentCalibration.Inductive),
            () => _ResistanceCoarseFixed = true
        );

    /// <summary>
    /// Calibrate the impedance pair.
    /// </summary>
    /// <param name="valueDeviation">Deviation measured.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustImpedance(GoalDeviation valueDeviation)
        => AdjustCoarseAndFine(
            _ImpedanceCoarseFixed,
            valueDeviation,
            CurrentCalibration.Inductive,
            deviation => deviation.DeltaFactor,
            calibrationPair => new(CurrentCalibration.Resistive, calibrationPair),
            () => _ImpedanceCoarseFixed = true
        );

    /// <summary>
    /// Calibrate a pair of values.
    /// </summary>
    /// <param name="coarseAlreadyFixed">Set if the coarse value is already fixed.</param>
    /// <param name="valueDeviation">Measured deviation from the goal.</param>
    /// <param name="calibration">Calibration pair to process.</param>
    /// <param name="getDeviationField">Access the deviation to inspect.</param>
    /// <param name="createCalibration">Method to create a full calibration set to get a new measurement.</param>
    /// <param name="setCoarseAlreadyFixed">Set to fix the coarse calibration.</param>
    /// <returns>null if no futher processing is possible.</returns>
    private async Task<CalibrationStep?> AdjustCoarseAndFine(
        bool coarseAlreadyFixed,
        GoalDeviation valueDeviation,
        CalibrationPair calibration,
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
            var coraseStep = await AdjustCoarseOrFine(valueDeviation, getDeviationField, calibration.ChangeCoarse, createCalibration);

            if (coraseStep != null)
            {
                // We found a better match.
                if (coraseStep.CalibrationChanged) return coraseStep;

                // There is no better match in coarse, so fix the calibration value and try the fine value.
                setCoarseAlreadyFixed();
            }
        }

        // Adjust fine correction.
        var fineStep = await AdjustCoarseOrFine(valueDeviation, getDeviationField, calibration.ChangeFine, createCalibration);

        // If fine correction step makes it worse just skip it.
        return fineStep?.CalibrationChanged == true ? fineStep : null;
    }

    /// <summary>
    /// Adjust a single part (e.g. GoalValue.ApparentPower) of a single pair (e.g. Calibration.Resistive).
    /// </summary>
    /// <param name="valueDeviation">Deviations measured.</param>
    /// <param name="getDeviationField">Access the deviation information to use.</param>
    /// <param name="updateCalibrationValue">Update the calibration for the next step.</param>
    /// <param name="createCalibration">Create a complete calibration information for value measurement.</param>
    /// <returns>null if no further processing is possible, elsewhere the Calibration memember will be 
    /// null as well if the changed calibration moved us further away from the goal.</returns>
    private async Task<CalibrationStep?> AdjustCoarseOrFine(
        GoalDeviation valueDeviation,
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
        var nextValues = await hardware.MeasureAsync(nextCalibration);
        var nextDelta = nextValues / Goal;

        // There was some improvment.
        if (Math.Abs(getDeviationField(nextDelta)) < Math.Abs(deviation))
            return new()
            {
                Calibration = nextCalibration,
                Deviation = nextDelta,
                TotalAbsDelta = Math.Abs(nextDelta.DeltaFactor) + Math.Abs(nextDelta.DeltaPower),
                Values = nextValues,
            };

        // We made it worse or nothing changed at all - but indicated with Calibration null that we at least gave it a try.
        return new() { Calibration = null!, Values = null!, Deviation = null!, TotalAbsDelta = 0 };
    }
}