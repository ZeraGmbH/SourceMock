using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private static readonly MT310s2Functions[] MT310s2DCReferenceMeters = [MT310s2Functions.DCReferenceMeter1, MT310s2Functions.DCReferenceMeter2];
    private void AddMt310s2Probes()
    {
        /* MT310s2 */
        var functions = _request.Configuration.MT310s2Functions.ToHashSet();

        if (functions.Contains(MT310s2Functions.EMobReferenceMeter))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.MTS310s2EMob,
                EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(MT310s2Functions.EMobReferenceMeter)
            });

        foreach (var refMeter in MT310s2DCReferenceMeters)
            if (functions.Contains(refMeter))
                _probes.Add(new IPProbe
                {
                    Protocol = IPProbeProtocols.MTS310s2DCSource,
                    EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(refMeter)
                });

        if (functions.Contains(MT310s2Functions.DCCalibration))
            _probes.Add(new IPProbe
            {
                Protocol = IPProbeProtocols.MTS310s2Calibration,
                EndPoint = IPProtocolProvider.GetMT310s2FunctionEndpoint(MT310s2Functions.DCCalibration)
            });
    }
}
