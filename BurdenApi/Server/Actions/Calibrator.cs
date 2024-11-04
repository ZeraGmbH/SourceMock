using BurdenApi.Models;

namespace BurdenApi.Actions;

/// <summary>
/// Implements one burden calibration algorithm.
/// </summary>
public class Calibrator : ICalibrator
{
    /// <inheritdoc/>
    public Calibration InitialCalibration { get; private set; } = null!;

    /// <inheritdoc/>
    public GoalValue Goal { get; private set; } = null!;

    private Calibration _Calibration = null!;

    private ICalibrationHardware _Hardware = null!;

    private bool _ResistanceCoarseFixed = false;

    private bool _ImpedanceCoarseFixed = false;

    private readonly List<CalibrationStep> _Steps = [];

    /// <inheritdoc/>
    public CalibrationStep[] Steps => [.. _Steps];

    /// <inheritdoc/>
    public CalibrationStep? LastStep => _Steps.LastOrDefault();

    /// <inheritdoc/>
    public async Task RunAsync(GoalValue target, Calibration initial, ICalibrationHardware hardware)
    {
        _Hardware = hardware;

        _Calibration = initial;
        Goal = target;
        InitialCalibration = initial;

        _ImpedanceCoarseFixed = false;
        _ResistanceCoarseFixed = false;

        _Steps.Clear();

        for (var done = new HashSet<CalibrationStep>(); ;)
        {
            // Try to make it better.
            var step = await IterateAsync();

            if (step?.Calibration == null) break;

            // Already did this one
            if (!done.Add(step))
            {
                // Already fixed anything - must abort now.
                if (_ImpedanceCoarseFixed && _ResistanceCoarseFixed) break;

                // Find the best fit.
                step = _Steps.MinBy(s => s.TotalAbsDelta)!;

                // Simulate as latest.
                _Steps.Add(step);

                // Only fine tuning for now.
                _ImpedanceCoarseFixed = true;
                _ResistanceCoarseFixed = true;
            }

            // Remember new iteration.
            _Calibration = step.Calibration!;

            // Add to list.
            _Steps.Add(step);
        }
    }

    private async Task<CalibrationStep?> AdjustCoarseOrFine(GoalDeviation delta, Func<GoalDeviation, double> field, Func<int, CalibrationPair?> changer, Func<CalibrationPair, Calibration> createCalibration)
    {
        // Apply correction.
        var nextPair = changer(field(delta) > 0 ? -1 : +1);

        // Correction may not be possible - bounds are reached.
        if (nextPair == null) return null;

        // Re-measure.
        var nextCalibration = createCalibration(nextPair);
        var newValues = await _Hardware.MeasureAsync(nextCalibration);
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

    private async Task<CalibrationStep?> AdjustCoarseAndFine(bool coarseFixed, GoalDeviation delta, CalibrationPair pair, Func<GoalDeviation, double> field, Func<CalibrationPair, Calibration> createCalibration, Action<bool> setAlreadyFixed)
    {
        // No difference at all.
        if (field(delta) == 0) return null;

        // Adjust coarse corrextion.
        if (!coarseFixed)
        {
            var coraseStep = await AdjustCoarseOrFine(delta, field, pair.ChangeCoarse, createCalibration);

            if (coraseStep != null)
                if (coraseStep.Calibration != null)
                    return coraseStep;
                else
                    setAlreadyFixed(true);
        }

        // Adjust fine correction.
        var fineStep = await AdjustCoarseOrFine(delta, field, pair.ChangeFine, createCalibration);

        if (fineStep?.Calibration != null) return fineStep;

        // Nothing found.
        return null;
    }

    private Task<CalibrationStep?> AdjustResistance(GoalDeviation delta)
        => AdjustCoarseAndFine(_ResistanceCoarseFixed, delta, _Calibration.Resistive, d => d.DeltaPower, resPair => new(resPair, _Calibration.Inductive), f => _ResistanceCoarseFixed = f);

    private Task<CalibrationStep?> AdjustImpedance(GoalDeviation delta)
        => AdjustCoarseAndFine(_ImpedanceCoarseFixed, delta, _Calibration.Inductive, d => d.DeltaFactor, impPair => new(_Calibration.Resistive, impPair), f => _ImpedanceCoarseFixed = f);

    private async Task<CalibrationStep?> IterateAsync()
    {
        // Retrieve new values and relativ deviation from goal.
        var delta = await _Hardware.MeasureAsync(_Calibration) / Goal;

        // Work on the largest deviation first.
        var step =
            Math.Abs(delta.DeltaPower) > Math.Abs(delta.DeltaFactor)
            ? await AdjustResistance(delta) ?? await AdjustImpedance(delta)
            : Math.Abs(delta.DeltaPower) < Math.Abs(delta.DeltaFactor)
            ? await AdjustImpedance(delta) ?? await AdjustResistance(delta)
            : null;

        // See if we did anything.
        return step?.Calibration != null ? step : null;
    }
}