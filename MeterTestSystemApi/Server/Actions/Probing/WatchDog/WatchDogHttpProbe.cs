using MeterTestSystemApi.Actions.Probing.HTTP;
using MeterTestSystemApi.Services;
using WatchDogApi.Models;

namespace MeterTestSystemApi.Actions.Probing.WatchDog;

/// <summary>
/// 
/// </summary>
public class WatchDogHttpProbe(IWatchDogExecuter executor) : IHttpProbeExecutor
{
    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(HttpProbe probe)
    {
        try
        {
            var uri = executor.BuildHttpEndpointList($"{probe.EndPoint}", 2)[0];
            var avail = await executor.QueryWatchDogSingleEndpointAsync(uri);

            return new() { Succeeded = avail, Message = avail ? "WatchDog available" : "Bad Response from WatchDog" };
        }
        catch (Exception e)
        {
            return new() { Succeeded = false, Message = e.Message };
        }
    }
}