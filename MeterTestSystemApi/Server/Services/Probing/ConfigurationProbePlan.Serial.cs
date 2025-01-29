using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private static readonly SerialPortTypes[] SerialPorts = [SerialPortTypes.USB, SerialPortTypes.RS232];

    private void AddSerialProbes()
    {
        for (var i = 0; i < _request.SerialPorts.Count; i++)
        {
            var types = _request.SerialPorts[i].ToHashSet();

            foreach (var type in SerialPorts)
                if (types.Contains(type))
                {
                    if (_request.Configuration.FrequencyGenerator != null)
                        _probes.Add(new SerialProbe()
                        {
                            Protocol = SerialProbeProtocols.FG30x,
                            Device = new() { Type = type, Index = (uint)i }
                        });

                    if (_request.Configuration.MT768 != null)
                        _probes.Add(new SerialProbe()
                        {
                            Protocol = SerialProbeProtocols.MT768,
                            Device = new() { Type = type, Index = (uint)i }
                        });

                    if (_request.Configuration.PM8121ZIF != null)
                        _probes.Add(new SerialProbe()
                        {
                            Protocol = SerialProbeProtocols.PM8181,
                            Device = new() { Type = type, Index = (uint)i }
                        });

                    if (_request.Configuration.ESxB != null)
                        _probes.Add(new SerialProbe()
                        {
                            Protocol = SerialProbeProtocols.ESxB,
                            Device = new() { Type = type, Index = (uint)i }
                        });
                }
        }
    }

}
