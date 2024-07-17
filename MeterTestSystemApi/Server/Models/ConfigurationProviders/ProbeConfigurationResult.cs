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
    public List<string> Log { get; set; } = [];

    /// <summary>
    /// The configuration according to the requested operation.
    /// </summary>
    public MeterTestSystemComponentsConfiguration? Configuration { get; set; }
}