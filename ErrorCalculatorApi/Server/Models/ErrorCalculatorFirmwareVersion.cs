using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Firmware information on an error calculator.
/// </summary>
[Serializable]
public class ErrorCalculatorFirmwareVersion
{
    /// <summary>
    /// Model of the calculator.
    /// </summary>
    [Required, NotNull]
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// Current firmware version.
    /// </summary>
    [Required, NotNull]
    public string Version { get; set; } = null!;
}
