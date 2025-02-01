using ErrorCalculatorApi.Models;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    private async Task ProbeTcpIpAsync(IPProbe probe)
    {
        /* Not in plan. */
        if (probe.Result != ProbeResult.Planned) return;

        try
        {
            /* Configuration to use for probing - little tewak to make developers like us.. */
            var effectiveProbe = _useLocalhost
                ? new()
                {
                    EndPoint = new() { Server = Environment.GetEnvironmentVariable("HOSTNAME")!, Port = probe.EndPoint.Port },
                    Protocol = probe.Protocol,
                    Result = probe.Result,
                    ServerType = probe.ServerType,
                    TestPosition = probe.TestPosition
                }
                : probe;

            /* Use manual executor to probe the port. */
            var executer = _services.GetRequiredKeyedService<IProbeExecutor>(effectiveProbe.GetType());
            var info = await executer.ExecuteAsync(effectiveProbe);

            /* Copy result. */
            probe.Result = info.Succeeded ? ProbeResult.Succeeded : ProbeResult.Failed;

            /* Done if not found - typically will not end up here but throw some exception. */
            if (!info.Succeeded)
            {
                _logger.LogInformation("{Probe} failed: {Exception}", probe.ToString(), info.Message);

                return;
            }

            /* Global services. */
            switch (probe.Protocol)
            {
                case IPProbeProtocols.IPWatchdog:
                    _result.Configuration.EnableIPWatchDog = true;
                    break;
            }

            /* Not attached to a position. */
            var pos = (int)(probe.TestPosition ?? 0);

            if (pos-- < 1) return;

            /* Allocate test position. */
            var positions = _result.Configuration.TestPositions;

            while (positions.Count <= pos) positions.Add(new());

            /* STM needs some special considerations. */
            if (probe.ServerType.HasValue)
                foreach (var other in _probes)
                    if (other is IPProbe otherIp && otherIp.TestPosition == probe.TestPosition && otherIp.Result == ProbeResult.Planned)
                        if (otherIp.ServerType.HasValue && otherIp.ServerType != probe.ServerType)
                            otherIp.Result = ProbeResult.Skipped;

            /* MAD needs some special considerations. */
            if (probe.Protocol == IPProbeProtocols.MADServer1 || probe.Protocol == IPProbeProtocols.MADServer2)
                foreach (var other in _probes)
                    if (other is IPProbe otherIp && otherIp.TestPosition == probe.TestPosition && otherIp.Result == ProbeResult.Planned)
                        if (otherIp.Protocol != probe.Protocol)
                            otherIp.Result = ProbeResult.Skipped;

            /* Register the probing result. */
            switch (probe.Protocol)
            {
                case IPProbeProtocols.MADServer1:
                case IPProbeProtocols.MADServer2:
                    positions[pos].EnableMAD = true;
                    positions[pos].MadProtocol = ErrorCalculatorProtocols.MAD_1;
                    positions[pos].STMServer = probe.ServerType ?? ServerTypes.STM6000;
                    break;
            }
        }
        catch (Exception e)
        {
            /* Something went very wrong. */
            probe.Result = ProbeResult.Failed;

            _logger.LogError("probe {Probe} failed: {Exception}", probe.ToString(), e.Message);
        }
    }
}
