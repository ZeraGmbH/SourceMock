namespace BurdenApi.Models;

/// <summary>
/// Parameters to run the calibration for a single step.
/// </summary>
public class CalibrationRequest
{
    /// <summary>
    /// Target to find.
    /// </summary>
    public required GoalValue Goal { get; set; }

    /// <summary>
    /// Initial calibration values.
    /// </summary>
    public required Calibration InitialCalibration { get; set; }
}