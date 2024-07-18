using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// Describes a probing for components.
/// </summary>
public class ProbeConfigurationRequest
{
    /// <summary>
    /// Overall configuration to use.
    /// </summary>
    [NotNull, Required]
    public MeterTestSystemComponentsConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// Serial ports to scan.
    /// </summary>
    [NotNull, Required]
    public List<List<SerialPortTypes>> SerialPorts { get; set; } = [];

    /// <summary>
    /// HID events to scan.
    /// </summary>
    [NotNull, Required]
    public List<bool> HIDEvents { get; set; } = [];
}