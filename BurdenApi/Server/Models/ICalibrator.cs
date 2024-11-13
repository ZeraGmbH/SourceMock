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
    /// <param name="request">Configuration of the calibration algorithm.</param>
    Task RunAsync(CalibrationRequest request);

    /// <summary>
    /// Process a calibration, write the result back to the burden and
    /// validate at 80% and 120%.
    /// </summary>
    /// <param name="request">Configuration of the calibration algorithm.</param>
    /// <returns>Details on the calibration.</returns>
    Task<CalibrationStep[]> CalibrateStepAsync(CalibrationRequest request);
}
