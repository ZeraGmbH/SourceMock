namespace BurdenApi.Models;

/// <summary>
/// Describes a calibrator algorithm.
/// </summary>
public interface ICalibrator
{
    /// <summary>
    /// Initial calibration used to calibrate.
    /// </summary>
    Calibration InitialCalibration { get; }

    /// <summary>
    /// Target of the calibration.
    /// </summary>
    GoalValue Goal { get; }

    /// <summary>
    /// Report all steps needed to reach the goal.
    /// </summary>
    CalibrationStep[] Steps { get; }

    /// <summary>
    /// Report the final step of the calibration holding the result as well.
    /// </summary>
    CalibrationStep? LastStep { get; }

    /// <summary>
    /// Process a calibration.
    /// </summary>
    /// <param name="target">Target to find.</param>
    /// <param name="initial">Initial calibration values.</param>
    Task RunAsync(GoalValue target, Calibration initial);
}
