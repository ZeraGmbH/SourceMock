using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// Implements one burden calibration algorithm.
/// </summary>
public class Calibrator(ICalibrationHardware hardware, IInterfaceLogger logger, IServiceProvider services) : ICalibrator, ICalibrationContext
{
    /// <summary>
    /// Maximum voltage used when measuring the upper bound of a current burden.
    /// </summary>
    private const double CurrentBurdenVoltageLimit = 89d;

    /// <summary>
    /// Currents are measured in 1% of the range value.
    /// </summary>
    private const double CurrentBaseFactor = 0.1;

    /// <summary>
    /// Allowed factors when measuring the upper limits for currents.
    /// </summary>
    private static readonly List<double> _SupportedCurrentFactors = [0.01, 0.05, 0.1, 0.2, 0.5, 1, 1.5, 2];

    /// <inheritdoc/>
    public GoalValue Goal { get; private set; } = null!;

    /// <inheritdoc/>
    public GoalValue EffectiveGoal { get; private set; } = null!;

    /// <inheritdoc/>
    public Calibration CurrentCalibration => LastStep.Calibration;

    private readonly List<CalibrationStep> _Steps = [];

    /// <inheritdoc/>
    public CalibrationStep[] Steps => [.. _Steps];

    /// <inheritdoc/>
    public CalibrationStep LastStep => _Steps.Last();

    private Frequency _Frequency;

    /// <inheritdoc/>
    public Task<RefMeterValueWithQuantity> MeasureAsync(Calibration calibration) => hardware.MeasureAsync(calibration, _VoltageNotCurrent);

    /// <inheritdoc/>
    public Task<GoalValueWithQuantity> MeasureBurdenAsync() => hardware.MeasureBurdenAsync(_VoltageNotCurrent);

    private ICalibrationAlgorithm _Algorithm = null!;

    private readonly HashSet<Calibration> _CycleTester = [];

    /// <summary>
    /// Indicate if we are working with a voltage and not a current burden.
    /// </summary>
    private bool _VoltageNotCurrent;

    /// <summary>
    /// The range used in the loadpoint.
    /// </summary>
    private double _PreparedRange;

    /// <inheritdoc/>
    public GoalDeviation MakeDeviation(GoalValue values, GoalValue goal, double? range)
    {
        // Make sure we are working relative to the really measures range,
        if (range.HasValue)
            goal = new()
            {
                ApparentPower = goal.ApparentPower * (range.Value * range.Value) / (_PreparedRange * _PreparedRange),
                PowerFactor = goal.PowerFactor
            };

        // Calculate from goal.
        return values / goal;
    }

    /// <inheritdoc/>
    public async Task RunAsync(bool voltageNotCurrent, CalibrationRequest request, CancellationToken cancel)
    {
        _VoltageNotCurrent = voltageNotCurrent;

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

        // Reset state - caller is responsible to synchronize access.
        _Algorithm = services.GetRequiredKeyedService<ICalibrationAlgorithm>(request.Algorithm);

        var initialCalibration = _Algorithm.CreateInitialCalibration(
            await burden.GetCalibrationAsync(request.Burden, request.Range, request.Step, logger) ?? throw new ArgumentException("step not enabled to calibrate", nameof(request)));

        _Steps.Clear();
        _CycleTester.Clear();

        // Correct the goal in ANSI mode.
        EffectiveGoal = request.Burden != "ANSI"
            ? Goal
            : new()
            {
                ApparentPower = 1.025 * Goal.ApparentPower,
                PowerFactor = Goal.PowerFactor
            };

        // Correct the goal for current burden.
        var factor = _VoltageNotCurrent ? 1 : CurrentBaseFactor;

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
        _PreparedRange = (await hardware.PrepareAsync(_VoltageNotCurrent, request.Range, factor, _Frequency, request.ChooseBestRange, Goal.ApparentPower)).Item2;

        // Switch burden on.
        await burden.SetBurdenAsync(request.Burden, logger);
        await burden.SetRangeAsync(request.Range, logger);
        await burden.SetStepAsync(request.Step, logger);
        await burden.SetActiveAsync(true, logger);

        // Create initial step.
        var values = await MeasureAsync(initialCalibration);
        var deviation = MakeDeviation(values, EffectiveGoal, values.Range);
        var burdenValues = await MeasureBurdenAsync();

        _Steps.Add(new()
        {
            BurdenDeviation = MakeDeviation(burdenValues, EffectiveGoal, values.Range),
            BurdenValues = burdenValues,
            Calibration = initialCalibration,
            Deviation = deviation,
            Factor = factor,
            Values = values
        });

        // Secure bound by a maximum number of steps.
        while (_Steps.Count < 1000)
        {
            // Check for abort.
            cancel.ThrowIfCancellationRequested();

            // Try to make it better.
            var step = await _Algorithm.IterateAsync(this);

            // We found no way to improve the calibration.
            if (step == null) break;

            // Already did this one?
            if (!_CycleTester.Add(step.Calibration))
            {
                // Already fixed anything - must abort now.
                if (!_Algorithm.ContinueAfterCycleDetection()) break;

                // Add to list.
                _Steps.Add(step);

                // Find the best fit - will become the latest result.
                step = _Steps.MinBy(s => s.TotalAbsDelta)!;
            }

            // Add to list.
            _Steps.Add(step);

            // Just to give a hint on how many work we did.
            step.Iteration = _Steps.Count;
        }

        // Finish result.
        LastStep.Factor = factor;
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
    public async Task<CalibrationStep[]> CalibrateStepAsync(bool voltageNotCurrent, CalibrationRequest request, CancellationToken cancel)
    {
        // Single measurement.
        await RunAsync(voltageNotCurrent, request, cancel);

        // See if it worked.
        var result = LastStep;

        if (result == null) return [];

        // Write to burden.
        await hardware.Burden.SetPermanentCalibrationAsync(request.Burden, request.Range, request.Step, result.Calibration, logger);

        // Measure at 80% (voltage) or 1% (current).
        var goalCorrection = voltageNotCurrent ? 1 : CurrentBaseFactor;
        var lowerFactor = voltageNotCurrent ? 0.8 : 0.01;

        var lowerPrepare = await hardware.PrepareAsync(voltageNotCurrent, request.Range, lowerFactor, _Frequency, request.ChooseBestRange, Goal.ApparentPower, false);

        lowerFactor = lowerPrepare.Item1;

        var lower = await MeasureAsync(result.Calibration);
        var lowerValues = await MeasureBurdenAsync();
        var lowerGoal = MakeEffectiveGoal(lowerFactor / goalCorrection);
        var lowerDeviation = MakeDeviation(lower, lowerGoal, lower.Range);

        // Measure at 120% (voltage) or 200% (current, voltage limited to 89V).
        var upperFactor = voltageNotCurrent ? 1.2 : 2;

        // Check for voltage limit.
        if (!voltageNotCurrent)
        {
            // Nominal current range to use, e.g. 0.5A.
            var range = BurdenUtils.ParseRange(request.Range);

            // The current of the measurement, e.g. 2 * 0.5A = 1A;
            var current = upperFactor * range;

            // Considering the apparent power (e.g. 4 * 25VA = 100VA) the correspoing voltage, e.g. 100V.
            var voltage = (double)(Goal.ApparentPower * upperFactor * upperFactor) / current;

            // Choose factor so that maximum apparent power is used, e.g. 1.78 leading to a current of 0.89A and a voltage of 89V giving an apparent power of 79.21VA.  
            if (voltage > CurrentBurdenVoltageLimit)
            {
                upperFactor = CurrentBurdenVoltageLimit * range / (double)Goal.ApparentPower;

                // Find the best match in the supported list - e.g. 1.5 leading to a current of 0.75A and a voltage of 75V giving an apparent power of 56.25VA.  
                var best = _SupportedCurrentFactors.FindLastIndex(f => f <= upperFactor);

                // Very defensive - never expect a factor below 0.01.
                if (best >= 0) upperFactor = _SupportedCurrentFactors[best];
            }
        }

        var upperPrepare = await hardware.PrepareAsync(voltageNotCurrent, request.Range, upperFactor, _Frequency, request.ChooseBestRange, Goal.ApparentPower, false);

        upperFactor = upperPrepare.Item1;

        var upper = await MeasureAsync(result.Calibration);
        var upperValues = await MeasureBurdenAsync();
        var upperGoal = MakeEffectiveGoal(upperFactor / goalCorrection);
        var upperDeviation = MakeDeviation(upper, upperGoal, upper.Range);

        // Construct result.
        return [
            result,
            new() {
                BurdenDeviation = MakeDeviation(lowerValues, lowerGoal, lower.Range),
                BurdenValues = lowerValues,
                Calibration = result.Calibration,
                Deviation = lowerDeviation,
                Factor = lowerFactor,
                Values = lower,
            },
            new() {
                BurdenDeviation = MakeDeviation(upperValues, upperGoal, upper.Range),
                BurdenValues = upperValues,
                Calibration = result.Calibration,
                Deviation = upperDeviation,
                Factor = upperFactor,
                Values = upper,
            }
        ];
    }

    /// <inheritdoc/>
    public void ClearCycleTester() => _CycleTester.Clear();
}