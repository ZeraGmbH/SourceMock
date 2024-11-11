using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;

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
    /// Set to choose the best voltage or current range.
    /// </summary>
    [NotNull, Required]
    public bool ChooseBestRange { get; set; }

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

    /// <summary>
    /// Percentage of the range to use - a factor with 1 meaing
    /// to use the exact value of the range in the loadpoint.
    /// </summary>
    [NotNull, Required]
    public double Percentage { get; set; }

    /// <summary>
    /// Set to find the best fit range of the reference meter.
    /// </summary>
    [NotNull, Required]
    public bool FindBestReferenceMeterRange { get; set; }
}