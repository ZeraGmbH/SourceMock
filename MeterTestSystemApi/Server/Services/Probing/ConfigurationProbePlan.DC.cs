using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
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

    private void AddDcProbes()
    {
        /* DC test system. */
        var dcComponents = _request.Configuration.DCComponents.ToHashSet();

        foreach (var dcCurrent in DCCurrents)
            if (dcComponents.Contains(dcCurrent))
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.DCCurrent,
                    EndPoint = IPProtocolProvider.GetDCComponentEndpoint(dcCurrent)
                });

        foreach (var dcVoltage in DCCVoltages)
            if (dcComponents.Contains(dcVoltage))
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.DCVoltage,
                    EndPoint = IPProtocolProvider.GetDCComponentEndpoint(dcVoltage)
                });

        if (dcComponents.Contains(DCComponents.SPS))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.DCSPS,
                EndPoint = IPProtocolProvider.GetDCComponentEndpoint(DCComponents.SPS)
            });

        if (dcComponents.Contains(DCComponents.FGControl))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.DCFGControl,
                EndPoint = IPProtocolProvider.GetDCComponentEndpoint(DCComponents.FGControl)
            });
    }
}
