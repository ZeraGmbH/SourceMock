namespace BurdenApi.Models;

/// <summary>
/// Describes a single iteration step during a calibration.
/// </summary>
public class CalibrationStep
{
    /// <summary>
    /// Calibration settings.
    /// </summary>
    public required Calibration Calibration { get; set; }

    /// <summary>
    /// See if the current calibration should be kept and 
    /// the next level should be inspected.
    /// </summary>
    public bool CalibrationChanged => Calibration != null;

    /// <summary>
    /// Actual values for the calibration.
    /// </summary>
    public required GoalValue Values { get; set; }

    /// <summary>
    /// Deviation from the calibration goal.
    /// </summary>
    public required GoalDeviation Deviation { get; set; }

    /// <summary>
    /// Sum of the deviation totals as a mean to find better calibrations.
    /// </summary>
    public required double TotalAbsDelta { get; set; }

    /// <summary>
    /// Get a hashcode from the calibration data.
    /// </summary>
    /// <returns>Some hash code.</returns>
    public override int GetHashCode() => Calibration?.GetHashCode() ?? 0;

    /// <summary>
    /// Create a description for this step.
    /// </summary>
    public override string ToString() => $"{Calibration}: {Values} ({Deviation})";
}
