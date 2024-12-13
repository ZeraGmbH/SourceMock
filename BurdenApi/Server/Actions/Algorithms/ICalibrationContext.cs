using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// 
/// </summary>
public interface ICalibrationContext
{
    /// <summary>
    /// The current value of the calibration.
    /// </summary>
    Calibration CurrentCalibration { get; }

    /// <summary>
    /// Target of the calibration.
    /// </summary>
    GoalValue EffectiveGoal { get; }

    /// <summary>
    /// Last calibration step executed.
    /// </summary>
    CalibrationStep LastStep { get; }

    /// <summary>
    /// Get the current values from the reference meter.
    /// </summary>
    /// <param name="calibration">Calibration to apply to the burden before measuring.</param>
    /// <returns>Values as measured by the reference meter.</returns>
    Task<Tuple<GoalValue, double?>> MeasureAsync(Calibration calibration);

    /// <summary>
    /// Get the current values from the burden.
    /// </summary>
    /// <returns>Values as measured by the burden.</returns>
    Task<Tuple<GoalValue, double?>> MeasureBurdenAsync();

    /// <summary>
    /// Reset the cycle tester.
    /// </summary>
    void ClearCycleTester();

    /// <summary>
    /// Calculate the deviation.
    /// </summary>
    /// <param name="values">What we measured.</param>
    /// <param name="goal">Target to reach.</param>
    /// <param name="range">Measured physical quantity.</param>
    /// <returns>Relative deviation.</returns>
    GoalDeviation MakeDeviation(GoalValue values, GoalValue goal, double? range);
}

/// <summary>
/// 
/// </summary>
public static class ICalibrationContextExtensions
{
    /// <summary>
    /// Calculate the deviation.
    /// </summary>
    /// <param name="context">Calibration context to use.</param>
    /// <param name="values">What we measured.</param>
    /// <param name="range">Measured physical quantity.</param>
    /// <returns>Relative deviation.</returns>
    public static GoalDeviation MakeDeviation(this ICalibrationContext context, GoalValue values, double? range) => context.MakeDeviation(values, context.EffectiveGoal, range);
}