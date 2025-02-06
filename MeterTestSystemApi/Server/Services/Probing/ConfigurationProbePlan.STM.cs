using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZeraDevices.ErrorCalculator.STM;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private static readonly ServerTypes[] STMServerTypes = [ServerTypes.STM6000, ServerTypes.STM4000];

    private static readonly IPProbeProtocols[] MADVersions = [IPProbeProtocols.MADServer2, IPProbeProtocols.MADServer1];
    private void AddStmProbes()
    {
        /* Check for contraint. */
        var positionCount = (uint)_request.Configuration.TestPositions.Count;

        if (positionCount == 0) return;

        TestPositionConfiguration.AssertPosition(positionCount);

        /* Per test position probes - STM6000 and STM4000. */
        for (uint pos = 0; pos < positionCount;)
        {
            /* See if the position should be scanned. */
            var config = _request.Configuration.TestPositions[(int)pos++];

            if (!config.Enabled) continue;

            /* STM6000 and STM4000 */
            foreach (var type in STMServerTypes)
            {
                /* Check the concrete server type. */
                if (config.STMServer.HasValue && type != config.STMServer) continue;

                if (config.EnableMAD)
                    foreach (var version in MADVersions)
                    {
                        /* Must match protocol version. */
                        switch (config.MadProtocol)
                        {
                            case ErrorCalculatorProtocols.HTTP:
                                continue;
                            case ErrorCalculatorProtocols.MAD_1:
                                if (version == IPProbeProtocols.MADServer1) break;

                                /* Skip on mismatch. */
                                continue;
                            case null:
                                /* No constraint active. */
                                break;
                            default:
                                throw new NotImplementedException("bad MAD protocol version detected");
                        }

                        _probes.Add(new IPProbe
                        {
                            Protocol = version,
                            EndPoint = IPProtocolProvider.GetMadEndpoint(pos, type),
                            TestPosition = pos,
                            ServerType = type
                        });
                    }

                if (config.EnableUpdateServer)
                    _probes.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.UpdateServer,
                        EndPoint = IPProtocolProvider.GetUpdateEndpoint(pos, type),
                        TestPosition = pos,
                        ServerType = type
                    });

                if (config.EnableDirectDutConnection)
                    _probes.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.COMServerDUT,
                        EndPoint = IPProtocolProvider.GetDirectDutConnectionEndpoint(pos, type),
                        TestPosition = pos,
                        ServerType = type
                    });

                if (config.EnableUART)
                    _probes.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.COMServerUART,
                        EndPoint = IPProtocolProvider.GetUARTEndpoint(pos, type),
                        TestPosition = pos,
                        ServerType = type
                    });
            }

            /* STM6000 only. */
            if (config.STMServer.HasValue && config.STMServer != ServerTypes.STM6000) continue;

            if (config.EnableObjectAccess)
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServerObjectAccess,
                    EndPoint = IPProtocolProvider.GetObjectAccessEndpoint(pos, ServerTypes.STM6000),
                    TestPosition = pos
                });

            if (config.EnableCOMServer)
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServer,
                    EndPoint = IPProtocolProvider.GetCOMServerEndpoint(pos, ServerTypes.STM6000),
                    TestPosition = pos
                });

            if (config.EnableSIMServer1)
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.SIMServer1,
                    EndPoint = IPProtocolProvider.GetSIMServer1Endpoint(pos, ServerTypes.STM6000),
                    TestPosition = pos
                });

            if (config.EnableBackendGateway)
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.BackendGateway,
                    EndPoint = IPProtocolProvider.GetBackendGatewayEndpoint(pos, ServerTypes.STM6000),
                    TestPosition = pos
                });
        }
    }
}
