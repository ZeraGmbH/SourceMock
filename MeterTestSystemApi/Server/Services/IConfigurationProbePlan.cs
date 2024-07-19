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
    void ConfigureProbe(ProbeConfigurationRequest request);

    /// <summary>
    /// Finish the probe.
    /// </summary>
    /// <returns>Result of the probing operation.</returns>
    ProbeConfigurationResult FinishProbe();
}
