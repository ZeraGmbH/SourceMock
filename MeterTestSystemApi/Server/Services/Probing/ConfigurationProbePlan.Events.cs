namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private void AddHIDProbes()
    {
        for (var i = 0; i < _request.HIDEvents.Count; i++)
            if (_request.HIDEvents[i])
                if (_request.Configuration.BarcodeReader.HasValue)
                    _probes.Add(new HIDProbe()
                    {
                        Protocol = HIDProbeProtocols.Keyboard,
                        Index = (uint)i
                    });
    }
}
