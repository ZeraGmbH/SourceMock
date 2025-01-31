namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private void AddHIDProbes()
    {
        for (uint i = 0; i < _request.HIDEventCount; i++)
            if (_request.Configuration.BarcodeReader.HasValue)
                _probes.Add(new HIDProbe()
                {
                    Protocol = HIDProbeProtocols.Keyboard,
                    Index = _request.MinHIDEvent + i
                });
    }
}
