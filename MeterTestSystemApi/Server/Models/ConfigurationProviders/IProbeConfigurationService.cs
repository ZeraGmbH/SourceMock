using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// Service to probe the system for potential meter test system components.
/// </summary>
public interface IProbeConfigurationService
{
    /// <summary>
    /// Probe the system for meter test system components.
    /// </summary>
    /// <param name="request">Hints on the scan.</param>
    /// <param name="services">Scoped service provider.</param>
    Task ConfigureProbingAsync(ProbeConfigurationRequest request, IServiceProvider services);

    /// <summary>
    /// Start the probing on a separate thread.
    /// </summary>
    void StartProbing();

    /// <summary>
    /// Abort the current probe operation.
    /// </summary>
    void Abort();

    /// <summary>
    /// Report if a probe operation is currently active.
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Manual probe.
    /// </summary>
    /// <param name="probes">List of probes to execute.</param>
    /// <param name="services">Scoped service provider.</param>
    /// <returns>Empty or some error information.</returns>
    Task<ProbeInfo[]> ProbeManualAsync(IEnumerable<Probe> probes, IServiceProvider services);
}