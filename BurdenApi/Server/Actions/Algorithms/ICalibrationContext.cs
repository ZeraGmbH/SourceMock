using BurdenApi.Models;

namespace BurdenApi.Actions.Algorithms;

/// <summary>
/// 
/// /// </summary>
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
    /// Get the current values from the reference meter.
    /// </summary>
    /// <param name="calibration">Calibration to apply to the burden before measuring.</param>
    /// <returns>Values as measured by the reference meter.</returns>
    Task<GoalValue> MeasureAsync(Calibration calibration);

    /// <summary>
    /// Get the current values from the burden.
    /// </summary>
    /// <returns>Values as measured by the burden.</returns>
    Task<GoalValue> MeasureBurdenAsync();
}
