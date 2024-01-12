using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// Firmware information on a meter test system.
/// </summary>
[Serializable]
public class MeterTestSystemFirmwareVersion
{
    /// <summary>
    /// Model of the system.
    /// </summary>
    [Required, NotNull]
    public string ModelName { get; set; } = null!;

    /// <summary>
    /// Current firmware version.
    /// </summary>
    [Required, NotNull]
    public string Version { get; set; } = null!;
}
