using BurdenApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions;

/// <summary>
/// Implements one burden calibration algorithm.
/// </summary>
public class Calibrator(ICalibrationHardware hardware, IInterfaceLogger logger) : ICalibrator
{
    /// <summary>
    /// Maximum voltage used when measuring the upper bound of a current burden.
    /// </summary>
    private const double CurrentBurdenVoltageLimit = 89d;

    /// <summary>
    /// Allowed factors when measuring the upper limits for currents.
    /// </summary>
    private static readonly List<double> _SupportedCurrentFactors = [0.01, 0.05, 0.1, 0.2, 0.5, 1, 1.5, 2];

    /// <inheritdoc/>
    public Calibration InitialCalibration { get; private set; } = null!;

    /// <inheritdoc/>
    public GoalValue Goal { get; private set; } = null!;

    private GoalValue EffectiveGoal = null!;

    private Calibration CurrentCalibration => LastStep?.Calibration ?? InitialCalibration;

    private bool _ResistanceCoarseFixed = false;

    private bool _ImpedanceCoarseFixed = false;

    private readonly List<CalibrationStep> _Steps = [];

    /// <inheritdoc/>
    public CalibrationStep[] Steps => [.. _Steps];

    /// <inheritdoc/>
    public CalibrationStep? LastStep => _Steps.LastOrDefault();

    private Frequency _Frequency;

    /// <inheritdoc/>
    public async Task RunAsync(CalibrationRequest request, CancellationToken cancel)
    {
        // Caluclate the goal from the step.
        var parts = request.Step.Split(";");

        if (parts.Length != 2) throw new ArgumentException("bad step notation", nameof(request));

        Goal = new()
        {
            ApparentPower = new(double.Parse(parts[0])),
            PowerFactor = new(double.Parse(parts[1]))
        };

        // Read the initial configuration from the burden.
        var burden = hardware.Burden;
        var calibration = await burden.GetCalibrationAsync(request.Burden, request.Range, request.Step, logger) ?? throw new ArgumentException("step not enabled to calibrate", nameof(request));

        // Start just in the middle of the fine range.
        InitialCalibration = new(new(calibration.Resistive.Coarse, 64), new(calibration.Inductive.Coarse, 64));

        // Reset state - caller is responsible to synchronize access.
        _ImpedanceCoarseFixed = false;
        _ResistanceCoarseFixed = false;

        _Steps.Clear();

        // Correct the goal in ANSI mode.
        EffectiveGoal = request.Burden != "ANSI"
            ? Goal
            : new()
            {
                ApparentPower = 1.025 * Goal.ApparentPower,
                PowerFactor = Goal.PowerFactor
            };

        // Correct the goal for current burden.
        var burdenInfo = await burden.GetVersionAsync(logger);
        var factor = burdenInfo.IsVoltageNotCurrent ? 1 : 0.1;

        EffectiveGoal = MakeEffectiveGoal(factor);

        // Get the frequency from the burden.
        _Frequency = new(
            request.Burden switch
            {
                "IEC50" => 50,
                "IEC60" or "ANSI" => 60,
                _ => throw new ArgumentException($"can not get frequency from burden '{request.Burden}'", nameof(request))
            });

        // Initialize and switch off the burden.
        await burden.CancelCalibrationAsync(logger);
        await burden.SetMeasuringCalibrationAsync(false, logger);
        await burden.SetActiveAsync(false, logger);

        // Prepare the loadpoint for this step.
        await hardware.PrepareAsync(request.Range, factor, _Frequency, request.ChooseBestRange, Goal.ApparentPower);

        // Switch burden on.
        await burden.SetBurdenAsync(request.Burden, logger);
        await burden.SetRangeAsync(request.Range, logger);
        await burden.SetStepAsync(request.Step, logger);
        await burden.SetActiveAsync(true, logger);

        // Secure bound by a maximum number of steps - we expect a maximum of 4 * 127 steps.
        for (var done = new HashSet<CalibrationStep>(); _Steps.Count < 1000;)
        {
            // Check for abort.
            cancel.ThrowIfCancellationRequested();

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
        var deviation = await hardware.MeasureAsync(CurrentCalibration) / EffectiveGoal;

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
        var nextDelta = nextValues / EffectiveGoal;

        // We made it worse or nothing changed at all - but indicated with Calibration null that we at least gave it a try.
        if (Math.Abs(getDeviationField(nextDelta)) >= Math.Abs(deviation)) return new() { Calibration = null!, Values = null!, Deviation = null!, TotalAbsDelta = 0 };

        // Apply measurement values from the burden as well.
        var burdenValues = await hardware.MeasureBurdenAsync();
        var values = new GoalValue { ApparentPower = burdenValues.ApparentPower, PowerFactor = burdenValues.PowerFactor };

        return new()
        {
            BurdenDeviation = values / EffectiveGoal,
            BurdenValues = values,
            Calibration = nextCalibration,
            Deviation = nextDelta,
            TotalAbsDelta = Math.Abs(nextDelta.DeltaFactor) + Math.Abs(nextDelta.DeltaPower),
            Values = nextValues,
        };
    }

    /// <summary>
    /// Create a goal value from the step configuration and a scling factor.
    /// </summary>
    /// <param name="factor">Some scaling factor - 1 means keep as is.</param>
    /// <returns>Scaled goal.</returns>
    private GoalValue MakeEffectiveGoal(double factor)
        => new()
        {
            ApparentPower = EffectiveGoal.ApparentPower * factor * factor,
            PowerFactor = EffectiveGoal.PowerFactor
        };

    /// <inheritdoc/>
    public async Task<CalibrationStep[]> CalibrateStepAsync(CalibrationRequest request, CancellationToken cancel)
    {
        // Single measurement.
        await RunAsync(request, cancel);

        // See if it worked.
        var result = LastStep;

        if (result == null) return [];

        // Write to burden.
        await hardware.Burden.SetPermanentCalibrationAsync(request.Burden, request.Range, request.Step, result.Calibration, logger);

        // Check mode.
        var burdenInfo = await hardware.Burden.GetVersionAsync(logger);

        // Measure at 80% (voltage) or 1% (current).
        var factor = burdenInfo.IsVoltageNotCurrent ? 0.8 : 0.01;

        await hardware.PrepareAsync(request.Range, factor, _Frequency, request.ChooseBestRange, Goal.ApparentPower);

        var lower = await hardware.MeasureAsync(result.Calibration);
        var lowerValues = await hardware.MeasureBurdenAsync();
        var lowerDeviation = lower / MakeEffectiveGoal(factor);

        // Measure at 120% (voltage) or 200% (current, voltage limited to 89V).
        factor = burdenInfo.IsVoltageNotCurrent ? 1.2 : 2;

        // Check for voltage limit.
        if (!burdenInfo.IsVoltageNotCurrent)
        {
            // Nominal current range to use, e.g. 0.5A.
            var range = BurdenUtils.ParseRange(request.Range);

            // The current of the measurement, e.g. 2 * 0.5A = 1A;
            var current = factor * range;

            // Considering the apparent power (e.g. 4 * 25VA = 100VA) the correspoing voltage, e.g. 100V.
            var voltage = (double)(Goal.ApparentPower * factor * factor) / current;

            // Choose factor so that maximum apparent power is used, e.g. 1.78 leading to a current of 0.89A and a voltage of 89V giving an apparent power of 79.21VA.  
            if (voltage > CurrentBurdenVoltageLimit)
            {
                factor = CurrentBurdenVoltageLimit * range / (double)Goal.ApparentPower;

                // Find the best match in the supported list - e.g. 1.5 leading to a current of 0.75A and a voltage of 75V giving an apparent power of 56.25VA.  
                var best = _SupportedCurrentFactors.FindLastIndex(f => f <= factor);

                // Very defensive - never expect a factor below 0.01.
                if (best >= 0) factor = _SupportedCurrentFactors[best];
            }
        }

        await hardware.PrepareAsync(request.Range, factor, _Frequency, request.ChooseBestRange, Goal.ApparentPower);

        var upper = await hardware.MeasureAsync(result.Calibration);
        var upperValues = await hardware.MeasureBurdenAsync();
        var upperDeviation = upper / EffectiveGoal;

        // Construct result.
        return [
            result,
            new() {
                BurdenDeviation = lowerValues/EffectiveGoal,
                BurdenValues = lowerValues,
                Calibration = result.Calibration,
                Deviation = lowerDeviation,
                Iteration = 0,
                TotalAbsDelta=Math.Abs(lowerDeviation.DeltaFactor) + Math.Abs(lowerDeviation.DeltaPower),
                Values = lower,
            },
            new() {
                BurdenDeviation = upperValues/EffectiveGoal,
                BurdenValues = upperValues,
                Calibration = result.Calibration,
                Deviation = upperDeviation,
                Iteration = 0,
                TotalAbsDelta=Math.Abs(upperDeviation.DeltaFactor) + Math.Abs(upperDeviation.DeltaPower),
                Values = upper,
            }
        ];
    }
}