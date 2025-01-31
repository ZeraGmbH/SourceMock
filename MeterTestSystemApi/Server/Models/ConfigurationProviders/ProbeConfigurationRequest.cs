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
    /// Minimum index of HID event devices to respect.
    /// </summary>
    [NotNull, Required]
    public uint MinHIDEvent { get; set; }

    /// <summary>
    /// Number of HID event devices to respect.
    /// </summary>
    [NotNull, Required]
    public uint HIDEventCount { get; set; }

    /// <summary>
    /// If set the system configuration will not be modified.
    /// </summary>
    [NotNull, Required]
    public bool DryRun { get; set; }
}