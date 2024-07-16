using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// Gives hints for a component probe.
/// </summary>
public class ProbeConfigurationRequest
{
    /// <summary>
    /// Number of positions to test - may reduce probing time.
    /// </summary>
    public int NumberOfPositions { get; set; } = TestPositionConfiguration.MaxPosition;

    /// <summary>
    /// DC components to test.
    /// </summary>
    public DCComponents DCComponents { get; set; } = DCComponents.All;
}