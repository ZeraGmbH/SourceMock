using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Information on a calibration.
/// </summary>
public class CalibrationResult
{
    /// <summary>
    /// Result of a calibration as it can be used in reports.
    /// </summary>
    [NotNull, Required]
    public CalibrationRequest Request { get; set; } = new();

    /// <summary>
    /// Result of the calibration.
    /// </summary>
    [NotNull, Required]
    public List<CalibrationStep> Calibration { get; set; } = [];
}