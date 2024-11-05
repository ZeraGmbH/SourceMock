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
    public async Task RunAsync(GoalValue target, Calibration initial)
    {
        // Reset state - caller is responsible to synchronize access.
        Goal = target;
        InitialCalibration = initial;

        _ImpedanceCoarseFixed = false;
        _ResistanceCoarseFixed = false;

        _Steps.Clear();

        // Secure bound by a maximum number of steps - we expect a maximum of 4 * 127 steps.
        for (var done = new HashSet<CalibrationStep>(); _Steps.Count < 1000;)
        {
            // Try to make it better.
            var step = await IterateAsync();

            // We found no way to improve the calibration.
            if (step?.Calibration == null) break;

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
        }
    }

    /// <summary>
    /// Execute one calibration iteration step.
    /// </summary>
    /// <returns>Successfull calibration step or null.</returns>
    private async Task<CalibrationStep?> IterateAsync()
    {
        // Retrieve new values and relativ deviation from goal.
        var delta = await hardware.MeasureAsync(CurrentCalibration) / Goal;

        // Work on the largest deviation first.
        var step =
            Math.Abs(delta.DeltaPower) >= Math.Abs(delta.DeltaFactor)
            ? await AdjustResistance(delta) ?? await AdjustImpedance(delta)
            : await AdjustImpedance(delta) ?? await AdjustResistance(delta);

        // See if we did anything.
        return step?.Calibration != null ? step : null;
    }


    /// <summary>
    /// Calibrate the resistence pair.
    /// </summary>
    /// <param name="delta">Deviation measured.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustResistance(GoalDeviation delta)
        => AdjustCoarseAndFine(_ResistanceCoarseFixed, delta, CurrentCalibration.Resistive, d => d.DeltaPower, resPair => new(resPair, CurrentCalibration.Inductive), f => _ResistanceCoarseFixed = f);

    /// <summary>
    /// Calibrate the impedance pair.
    /// </summary>
    /// <param name="delta">Deviation measured.</param>
    /// <returns>Successfull calibration step or null.</returns>
    private Task<CalibrationStep?> AdjustImpedance(GoalDeviation delta)
        => AdjustCoarseAndFine(_ImpedanceCoarseFixed, delta, CurrentCalibration.Inductive, d => d.DeltaFactor, impPair => new(CurrentCalibration.Resistive, impPair), f => _ImpedanceCoarseFixed = f);

    /// <summary>
    /// Calibrate a pair of values.
    /// </summary>
    /// <param name="coarseFixed">Set if the coarse value is already fixed.</param>
    /// <param name="delta">Measured deviation from the goal.</param>
    /// <param name="pair">Calibration pair to process.</param>
    /// <param name="field">Access the deviation to inspect.</param>
    /// <param name="createCalibration">Method to create a full calibration set to get a new measurement.</param>
    /// <param name="setAlreadyFixed">Set to fix the coarse calibration.</param>
    /// <returns>null if no futher processing is possible.</returns>
    private async Task<CalibrationStep?> AdjustCoarseAndFine(bool coarseFixed, GoalDeviation delta, CalibrationPair pair, Func<GoalDeviation, double> field, Func<CalibrationPair, Calibration> createCalibration, Action<bool> setAlreadyFixed)
    {
        // No difference at all.
        if (field(delta) == 0) return null;

        // Adjust coarse corrextion if not already fixed.
        if (!coarseFixed)
        {
            // Check if there is a better calibration.
            var coraseStep = await AdjustCoarseOrFine(delta, field, pair.ChangeCoarse, createCalibration);

            if (coraseStep != null)
            {
                // We found a better match.
                if (coraseStep.Calibration != null) return coraseStep;

                // There is no better match in coarse, so fix the calibration value.
                setAlreadyFixed(true);
            }
        }

        // Adjust fine correction.
        var fineStep = await AdjustCoarseOrFine(delta, field, pair.ChangeFine, createCalibration);

        // If fine correction step makes it worse just skip it.
        return fineStep?.Calibration != null ? fineStep : null;
    }

    /// <summary>
    /// Adjust a single part (e.g. GoalValue.ApparentPower) of a single pair (e.g. Calibration.Resistive).
    /// </summary>
    /// <param name="delta">Deviations measured.</param>
    /// <param name="field">Access the deviation information to use.</param>
    /// <param name="changer">Update the calibration for the next step.</param>
    /// <param name="createCalibration">Create a complete calibration information for value measurement.</param>
    /// <returns>null if no further processing is possible, elsewhere the Calibration memember will be 
    /// null as well if the changed calibration moved us further away from the goal.</returns>
    private async Task<CalibrationStep?> AdjustCoarseOrFine(GoalDeviation delta, Func<GoalDeviation, double> field, Func<bool, CalibrationPair?> changer, Func<CalibrationPair, Calibration> createCalibration)
    {
        // No difference at all - access part (e.g. GoalValue.ApparentPower) using accessor method field
        if (field(delta) == 0) return null;

        // Apply correction - it is expected that the order of calibration values mirrors the order of measured values.
        var nextPair = changer(field(delta) < 0);

        // Correction may not be possible - bounds are reached.
        if (nextPair == null) return null;

        // Re-measure - rebuild calibration using pair (e.g Calibration.Resistive) keeping the other part.
        var nextCalibration = createCalibration(nextPair);
        var newValues = await hardware.MeasureAsync(nextCalibration);
        var newDelta = newValues / Goal;

        // There was some improvment - at least it did not get worse.
        if (Math.Abs(field(newDelta)) <= Math.Abs(field(delta)))
            return new()
            {
                Calibration = nextCalibration,
                Deviation = newDelta,
                TotalAbsDelta = Math.Abs(newDelta.DeltaFactor) + Math.Abs(newDelta.DeltaPower),
                Values = newValues,
            };

        // We made it worse.
        return new() { Calibration = null!, Values = null!, Deviation = null!, TotalAbsDelta = 0 };
    }
}