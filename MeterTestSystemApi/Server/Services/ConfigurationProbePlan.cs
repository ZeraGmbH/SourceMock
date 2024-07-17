using ErrorCalculatorApi.Models;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MeterTestSystemApi.Services;

internal class ConfigurationProbePlan
{
    private static readonly ServerTypes[] STMServerTypes = [ServerTypes.STM4000, ServerTypes.STM6000];

    private static readonly IPProbeProtocols[] MADVersions = [IPProbeProtocols.MADServer1, IPProbeProtocols.MADServer2];

    private static readonly DCComponents[] DCCurrents = [
        DCComponents.CurrentSCG06,
        DCComponents.CurrentSCG1000,
        DCComponents.CurrentSCG750,
        DCComponents.CurrentSCG8,
        DCComponents.CurrentSCG80,
    ];

    private static readonly DCComponents[] DCCVoltages = [
        DCComponents.VoltageSVG1200,
        DCComponents.VoltageSVG150,
    ];

    private static readonly TransformerComponents[] TransformerPhases = [
        TransformerComponents.STR260Phase1,
        TransformerComponents.STR260Phase2,
        TransformerComponents.STR260Phase3,
    ];

    private static readonly NBoxRouterTypes[] NBoxRouters = [NBoxRouterTypes.Prime, NBoxRouterTypes.G3];

    private static readonly MT310s2Functions[] MT310s2DCReferenceMeters = [MT310s2Functions.DCReferenceMeter1, MT310s2Functions.DCReferenceMeter2];

    private readonly MeterTestSystemComponentsConfiguration _request;

    public ConfigurationProbePlan(MeterTestSystemComponentsConfiguration request)
    {
        _request = request;

        AddTcpIpProbes();
    }

    private void AddTcpIpProbes()
    {
        AddStmProbes();
        AddDcProbes();
        AddTransformerProbes();
        AddNBoxProbes();
        AddMt310s2Probes();

        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.OmegaiBTHX,
            EndPoint = IPProtocolProvider.GetOmegaiBTHXEndpoint()
        });

        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.COM5003,
            EndPoint = IPProtocolProvider.GetCOM5003Endpoint()
        });

        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.IPWatchdog,
            EndPoint = IPProtocolProvider.GetIPWatchDogEndpoint()
        });

        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.DTS100,
            EndPoint = IPProtocolProvider.GetDTS100Endpoint()
        });
    }

    private void AddMt310s2Probes()
    {
        /* MT310s2 */
        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.MTS310s2EMob,
            EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(MT310s2Functions.EMobReferenceMeter)
        });

        foreach (var refMeter in MT310s2DCReferenceMeters)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.MTS310s2DCSource,
                EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(refMeter)
            });

        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.MTS310s2Calibration,
            EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(MT310s2Functions.DCCalibration)
        });

        /* Single instance. */
        TCPIP.Add(new IPProbe
        {
            Protocol = IPProbeProtocols.MP2020Control,
            EndPoint = IPProtocolProvider.Get2020ControlEndpoint()
        });
    }

    private void AddNBoxProbes()
    {
        /* NBox PLC Router. */
        foreach (var router in NBoxRouters)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.NBoxRouter,
                EndPoint = IPProtocolProvider.GetNBoxRouterEndpoint(router)
            });
    }

    private void AddTransformerProbes()
    {
        /* Transformers. */
        if ((_request.TransformerComponents & TransformerComponents.CurrentWM3000or1000) != 0)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerCurrent,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.CurrentWM3000or1000)
            });

        if ((_request.TransformerComponents & TransformerComponents.SPS) != 0)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerSPS,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.SPS)
            });

        foreach (var phase in TransformerPhases)
            if ((_request.TransformerComponents & phase) != 0)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.TransformerSTR260,
                    EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(phase)
                });

        if ((_request.TransformerComponents & TransformerComponents.VoltageWM3000or1000) != 0)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerVoltage,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.VoltageWM3000or1000)
            });
    }

    private void AddDcProbes()
    {
        /* DC test system. */
        foreach (var dcCurrent in DCCurrents)
            if ((_request.DCComponents & dcCurrent) != 0)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.DCCurrent,
                    EndPoint = IPProtocolProvider.GetDCComponentEndpoint(dcCurrent)
                });

        foreach (var dcVoltage in DCCVoltages)
            if ((_request.DCComponents & dcVoltage) != 0)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.DCVoltage,
                    EndPoint = IPProtocolProvider.GetDCComponentEndpoint(dcVoltage)
                });

        if ((_request.DCComponents & DCComponents.SPS) != 0)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.DCSPS,
                EndPoint = IPProtocolProvider.GetDCComponentEndpoint(DCComponents.SPS)
            });

        if ((_request.DCComponents & DCComponents.FGControl) != 0)
            TCPIP.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.DCFGControl,
                EndPoint = IPProtocolProvider.GetDCComponentEndpoint(DCComponents.FGControl)
            });
    }

    private void AddStmProbes()
    {
        /* Check for contraint. */
        var positionCount = _request.TestPositions.Count;

        if (positionCount == 0) return;

        TestPositionConfiguration.AssertPosition(positionCount);

        /* Per test position probes - STM6000 and STM4000. */
        for (var pos = 0; pos < positionCount;)
        {
            /* See if the position should be scanned. */
            var config = _request.TestPositions[pos++];

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

                        TCPIP.Add(new IPProbe
                        {
                            Protocol = version,
                            EndPoint = IPProtocolProvider.GetMadEndpoint(pos, type)
                        });
                    }

                if (config.EnableUpdateServer)
                    TCPIP.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.UpdateServer,
                        EndPoint = IPProtocolProvider.GetUpdateEndpoint(pos, type)
                    });

                if (config.EnableDirectDutConnection)
                    TCPIP.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.COMServerDUT,
                        EndPoint = IPProtocolProvider.GetDirectDutConnectionEndpoint(pos, type)
                    });

                if (config.EnableUART)
                    TCPIP.Add(new IPProbe
                    {
                        Protocol = IPProbeProtocols.COMServerUART,
                        EndPoint = IPProtocolProvider.GetUARTEndpoint(pos, type)
                    });
            }

            /* STM6000 only. */
            if (config.STMServer.HasValue && config.STMServer != ServerTypes.STM6000) continue;

            if (config.EnableObjectAccess)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServerObjectAccess,
                    EndPoint = IPProtocolProvider.GetObjectAccessEndpoint(pos, ServerTypes.STM6000)
                });

            if (config.EnableCOMServer)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.COMServer,
                    EndPoint = IPProtocolProvider.GetCOMServerEndpoint(pos, ServerTypes.STM6000)
                });

            if (config.EnableSIMServer1)
                TCPIP.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.SIMServer1,
                    EndPoint = IPProtocolProvider.GetSIMServer1Endpoint(pos, ServerTypes.STM6000)
                });

            if (config.EnableBackendGateway)
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
            request.Log.Add($"TCP/IP {probe}: {probe.Result}");
    }
}
