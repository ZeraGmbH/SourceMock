using RefMeterApi.Models;
using SourceApi.Model;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MeterTestSystemApi.Models;

/// <summary>
/// Capabilities of a meter test system.
/// </summary>
public class MeterTestSystemCapabilities
{
    /// <summary>
    /// List of supported voltage amplifiers.
    /// </summary>
    [NotNull, Required]
    public List<VoltageAmplifiers> SupportedVoltageAmplifiers { get; set; } = [];

    /// <summary>
    /// List of supported auxiliary voltage amplifiers.
    /// </summary>
    [NotNull, Required]
    public List<VoltageAuxiliaries> SupportedVoltageAuxiliaries { get; set; } = [];

    /// <summary>
    /// List of supported current amplifiers.
    /// </summary>
    [NotNull, Required]
    public List<CurrentAmplifiers> SupportedCurrentAmplifiers { get; set; } = [];

    /// <summary>
    /// List of supported auxiliary current amplifiers.
    /// </summary>
    [NotNull, Required]
    public List<CurrentAuxiliaries> SupportedCurrentAuxiliaries { get; set; } = [];

    /// <summary>
    /// List of supported reference meters.
    /// </summary>
    [NotNull, Required]
    public List<ReferenceMeters> SupportedReferenceMeters { get; set; } = [];
}
