using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Parameters to run the calibration for a single step.
/// </summary>
public class CalibrationRequest
{
    /// <summary>
    /// Burden to use.
    /// </summary>
    [NotNull, Required]
    public string Burden { get; set; } = null!;

    /// <summary>
    /// Range to use.
    /// </summary>
    [NotNull, Required]
    public string Range { get; set; } = null!;

    /// <summary>
    /// Step to use.
    /// </summary>
    [NotNull, Required]
    public string Step { get; set; } = null!;

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