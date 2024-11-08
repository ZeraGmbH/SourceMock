using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Parameters to run the calibration for a single step.
/// </summary>
public class CalibrationRequest
{
    /// <summary>
    /// Target to find.
    /// </summary>
    [NotNull, Required]
    public GoalValue Goal { get; set; } = new();

    /// <summary>
    /// Initial calibration values.
    /// </summary>
    [NotNull, Required]
    public Calibration InitialCalibration { get; set; } = new();
}