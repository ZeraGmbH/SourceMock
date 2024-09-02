using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Probing plan.
/// </summary>
public interface IConfigurationProbePlan
{
    /// <summary>
    /// Load the list of probes.
    /// </summary>
    /// <param name="request">Probe configuration.</param>
    Task ConfigureProbeAsync(ProbeConfigurationRequest request);

    /// <summary>
    /// Finish the probe.
    /// </summary>
    /// <returns>Result of the probing operation.</returns>
    Task<ProbeConfigurationResult> FinishProbeAsync();
}
