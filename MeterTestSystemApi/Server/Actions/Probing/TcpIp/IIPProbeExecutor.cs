using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing.TcpIp;

/// <summary>
/// 
/// </summary>
public interface IIPProbeExecutor
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Connection to use.</param>
    /// <returns>Error message or empty.</returns>
    Task<ProbeInfo> ExecuteAsync(IPProbe probe);
}