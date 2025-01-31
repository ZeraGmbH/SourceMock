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
            var uri = $"http://{probe.EndPoint}/cgi-bin/refreshpage1.asp";
            var avail = await executor.QueryWatchDogAsync(uri);

            return new() { Succeeded = avail, Message = avail ? "WatchDog available" : "Bad Response from WatchDog" };
        }
        catch (Exception e)
        {
            return new ProbeInfo { Succeeded = false, Message = e.Message };
        }
    }
}