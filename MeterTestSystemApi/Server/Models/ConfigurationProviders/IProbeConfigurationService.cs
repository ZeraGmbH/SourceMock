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
    Task StartProbe(ProbeConfigurationRequest request, bool dryRun);

    /// <summary>
    /// Abort the current probe operation.
    /// </summary>
    Task Abort();

    /// <summary>
    /// Get the result of the last probing.
    /// </summary>
    ProbeConfigurationResult? Result { get; }

    /// <summary>
    /// Report if a probe operation is currently active.
    /// </summary>
    bool IsActive { get; }
}