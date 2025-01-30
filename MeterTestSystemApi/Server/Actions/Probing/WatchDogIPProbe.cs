using MeterTestSystemApi.Services;
using WatchDogApi.Models;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// 
/// </summary>
public class WatchDogIPProbe(IWatchDogExecuter executor) : IIPProbeExecutor
{
    /// <inheritdoc/>
    public Task<ProbeInfo> ExecuteAsync(IPProbe probe)
    {
        try
        {
            throw new NotImplementedException("will eventually be implemented");
        }
        catch (Exception e)
        {
            return Task.FromResult(new ProbeInfo { Succeeded = false, Message = e.Message });
        }
    }
}