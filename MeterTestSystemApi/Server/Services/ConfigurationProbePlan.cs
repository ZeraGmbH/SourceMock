using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services;

internal class ConfigurationProbePlan
{
    private static readonly ServerTypes[] STMServerTypes = [ServerTypes.STM4000, ServerTypes.STM6000];

    private static readonly IPProbeProtocols[] MADVersions = [IPProbeProtocols.MADServer1, IPProbeProtocols.MADServer2];

    public ConfigurationProbePlan()
    {
        /* Per test position probes - STM6000 and STM4000. */
        for (var pos = 0; pos++ < TestPositionConfiguration.MaxPosition;)
        {
            /* STM6000 and STM4000 */
            foreach (var type in STMServerTypes)
            {
                foreach (var version in MADVersions)
                    TCPIP.Add(new IPProbe
                    {
                        Protocol = version,
                        EndPoint = IPProtocolProvider.GetMadEndpoint(pos, type)
                    });

                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.UpdateServer,
                    EndPoint = IPProtocolProvider.GetUpdateEndpoint(pos, type)
                });

                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServerDUT,
                    EndPoint = IPProtocolProvider.GetDirectDutConnectionEndpoint(pos, type)
                });


                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServerUART,
                    EndPoint = IPProtocolProvider.GetUARTEndpoint(pos, type)
                });
            }

            /* STM6000 only. */
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.COMServerObjectAccess,
                EndPoint = IPProtocolProvider.GetUARTEndpoint(pos, ServerTypes.STM6000)
            });

            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.COMServer,
                EndPoint = IPProtocolProvider.GetCOMServerEndpoint(pos, ServerTypes.STM6000)
            });

            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.SIMServer1,
                EndPoint = IPProtocolProvider.GetSIMServer1Endpoint(pos, ServerTypes.STM6000)
            });

            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.BackendGateway,
                EndPoint = IPProtocolProvider.GetBackendGatewayEndpoint(pos, ServerTypes.STM6000)
            });
        }
    }

    public List<IPProbe> TCPIP { get; set; } = [];

    public void CreateReport(ProbeConfigurationResult request)
    {
        /* TCP/IP probes. */
        foreach (var probe in TCPIP)
            request.Log.Add($"TCP/IP {probe}");
    }
}
