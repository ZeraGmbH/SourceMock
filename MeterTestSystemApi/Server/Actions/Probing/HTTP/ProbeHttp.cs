using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Actions.Probing.HTTP;

/// <summary>
/// 
/// </summary>
public class ProbeHttp(IServiceProvider services) : ProbeExecutor<HttpProbe>
{
    /// <inheritdoc/>
    protected override Task<ProbeInfo> OnExecuteAsync(HttpProbe probe)
        => services.GetRequiredKeyedService<IHttpProbeExecutor>(probe.Protocol).ExecuteAsync(probe);
}