using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Probing plan.
/// </summary>
public interface IConfigurationProbePlan
{
    /// <summary>
    /// Load the list of probes.
    /// </summary>
    Task<ProbingOperation> ConfigureProbeAsync();

    /// <summary>
    /// Finish the probe.
    /// </summary>
    /// <returns>Result of the probing operation.</returns>
    Task FinishProbeAsync();
}
