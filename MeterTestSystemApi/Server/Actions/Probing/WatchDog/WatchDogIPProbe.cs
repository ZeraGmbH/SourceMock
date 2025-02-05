using MeterTestSystemApi.Actions.Probing.TcpIp;
using MeterTestSystemApi.Services;
using WatchDogApi.Models;

namespace MeterTestSystemApi.Actions.Probing.WatchDog;

/// <summary>
/// 
/// </summary>
public class WatchDogIPProbe(IWatchDogExecuter executor) : IIPProbeExecutor
{
    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(IPProbe probe)
    {
        try
        {
            var uri = executor.BuildHttpEndpointList($"{probe.EndPoint.Server}:{probe.EndPoint.Port}", 2)[0];
            var avail = await executor.QueryWatchDogSingleEndpointAsync(uri, TimeSpan.FromSeconds(2));

            return new() { Succeeded = avail, Message = avail ? "WatchDog available" : "Bad Response from WatchDog" };
        }
        catch (Exception e)
        {
            return new ProbeInfo { Succeeded = false, Message = e.Message };
        }
    }
}