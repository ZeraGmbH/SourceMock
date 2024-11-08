using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BurdenApi.Models;

/// <summary>
/// Describes a single iteration step during a calibration.
/// </summary>
public class CalibrationStep
{
    /// <summary>
    /// Calibration settings.
    /// </summary>
    public Calibration Calibration { get; set; } = null!;

    /// <summary>
    /// See if the current calibration should be kept and 
    /// the next level should be inspected.
    /// </summary>
    [JsonIgnore]
    public bool CalibrationChanged => Calibration != null;

    /// <summary>
    /// Actual values for the calibration.
    /// </summary>
    [NotNull, Required]
    public GoalValue Values { get; set; } = new();

    /// <summary>
    /// Deviation from the calibration goal.
    /// </summary>
    [NotNull, Required]
    public GoalDeviation Deviation { get; set; } = new();

    /// <summary>
    /// Sum of the deviation totals as a mean to find better calibrations.
    /// </summary>
    [NotNull, Required]
    public double TotalAbsDelta { get; set; }

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
