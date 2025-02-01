using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

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
    /// <param name="cancel">Cancel the probing before it is done.</param>
    /// <returns>Result of the probing operation.</returns>
    Task<ProbingOperation> FinishProbeAsync(CancellationToken cancel);
}
