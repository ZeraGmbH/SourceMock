using MeterTestSystemApi.Services;
using WatchDogApi.Models;

namespace MeterTestSystemApi.Actions.Probing;

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
            var avail = await executor.QueryWatchDogAsync(probe.EndPoint);

            return new() { Succeeded = avail, Message = avail ? "WatchDog available" : "Bad Response from WatchDog" };
        }
        catch (Exception e)
        {
            return new() { Succeeded = false, Message = e.Message };
        }
    }
}