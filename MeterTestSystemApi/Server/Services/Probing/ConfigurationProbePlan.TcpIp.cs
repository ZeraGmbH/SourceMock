using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private void AddTcpIpProbes()
    {
        AddStmProbes();
        AddDcProbes();
        AddTransformerProbes();
        AddNBoxProbes();
        AddMt310s2Probes();

        if (_request.Configuration.EnableMP2020Control)
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.MP2020Control,
                EndPoint = IPProtocolProvider.Get2020ControlEndpoint()
            });

        if (_request.Configuration.EnableOmegaiBTHX)
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.OmegaiBTHX,
                EndPoint = IPProtocolProvider.GetOmegaiBTHXEndpoint()
            });

        if (_request.Configuration.EnableCOM5003)
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.COM5003,
                EndPoint = IPProtocolProvider.GetCOM5003Endpoint()
            });

        if (_request.Configuration.EnableIPWatchDog)
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.IPWatchdog,
                EndPoint = IPProtocolProvider.GetIPWatchDogEndpoint()
            });

        if (_request.Configuration.EnableDTS100)
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.DTS100,
                EndPoint = IPProtocolProvider.GetDTS100Endpoint()
            });
    }
}
