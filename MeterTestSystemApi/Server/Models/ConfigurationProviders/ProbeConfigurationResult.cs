using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// Result of a component probe.
/// </summary>
public class ProbeConfigurationResult
{
    /// <summary>
    /// More or less readable information on what was probed.
    /// </summary>
    [NotNull, Required]
    public List<string> Log { get; set; } = [];

    /// <summary>
    /// The configuration according to the requested operation.
    /// </summary>
    [NotNull, Required]
    public MeterTestSystemComponentsConfiguration Configuration { get; set; } = new();
}