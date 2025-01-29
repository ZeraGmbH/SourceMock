using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private static readonly NBoxRouterTypes[] NBoxRouters = [NBoxRouterTypes.Prime, NBoxRouterTypes.G3];

    private void AddNBoxProbes()
    {
        /* NBox PLC Router. */
        var routers = _request.Configuration.NBoxRouterTypes.ToHashSet();

        foreach (var router in NBoxRouters)
            if (routers.Contains(router))
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.NBoxRouter,
                    EndPoint = IPProtocolProvider.GetNBoxRouterEndpoint(router)
                });
    }
}
