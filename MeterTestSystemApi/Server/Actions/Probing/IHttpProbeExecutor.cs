using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// 
/// </summary>
public interface IHttpProbeExecutor
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Connectiojn to use.</param>
    /// <returns>Error message or empty.</returns>
    Task<ProbeInfo> ExecuteAsync(HttpProbe probe);
}