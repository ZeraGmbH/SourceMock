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
}