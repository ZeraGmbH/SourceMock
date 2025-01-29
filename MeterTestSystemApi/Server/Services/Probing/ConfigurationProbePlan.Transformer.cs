using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private static readonly TransformerComponents[] TransformerPhases = [
        TransformerComponents.STR260Phase1,
        TransformerComponents.STR260Phase2,
        TransformerComponents.STR260Phase3,
    ];

    private void AddTransformerProbes()
    {
        /* Transformers. */
        var components = _request.Configuration.TransformerComponents.ToHashSet();

        if (components.Contains(TransformerComponents.CurrentWM3000or1000))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerCurrent,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.CurrentWM3000or1000)
            });

        if (components.Contains(TransformerComponents.SPS))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerSPS,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.SPS)
            });

        foreach (var phase in TransformerPhases)
            if (components.Contains(phase))
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.TransformerSTR260,
                    EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(phase)
                });

        if (components.Contains(TransformerComponents.VoltageWM3000or1000))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.TransformerVoltage,
                EndPoint = IPProtocolProvider.GetTransformerComponentEndpoint(TransformerComponents.VoltageWM3000or1000)
            });
    }
}
