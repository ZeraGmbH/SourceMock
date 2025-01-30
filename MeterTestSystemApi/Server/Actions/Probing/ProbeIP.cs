using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// 
/// </summary>
public class ProbeIP(IServiceProvider services) : ProbeExecutor<IPProbe>
{
    /// <inheritdoc/>
    protected override Task<ProbeInfo> OnExecuteAsync(IPProbe probe)
        => services.GetRequiredKeyedService<IIPProbeExecutor>(probe.Protocol).ExecuteAsync(probe);
}