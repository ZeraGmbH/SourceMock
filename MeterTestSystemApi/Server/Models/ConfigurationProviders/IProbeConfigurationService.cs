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
    /// <param name="dryRun">Set to do only report what to probe.</param>
    /// <param name="services">Scoped service provider.</param>
    Task StartProbeAsync(ProbeConfigurationRequest request, bool dryRun, IServiceProvider services);

    /// <summary>
    /// Abort the current probe operation.
    /// </summary>
    Task AbortAsync();

    /// <summary>
    /// Get the result of the last probing.
    /// </summary>
    ProbeConfigurationResult? Result { get; }

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