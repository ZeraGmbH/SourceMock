using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    private async Task ProbeSerialAsync(SerialProbe probe)
    {
        /* Not in plan. */
        if (probe.Result != ProbeResult.Planned) return;

        try
        {
            /* Use manual executor to probe the port. */
            var executer = _services.GetRequiredKeyedService<IProbeExecutor>(probe.GetType());
            var info = await executer.ExecuteAsync(probe);

            /* Copy result. */
            probe.Result = info.Succeeded ? ProbeResult.Succeeded : ProbeResult.Failed;

            /* Done if not found - typically will not end up here but throw some exception. */
            if (!info.Succeeded)
            {
                _logger.LogInformation("{Probe} failed: {Exception}", probe.ToString(), info.Message);

                return;
            }

            /* Make sure neither this port nor this device is probed for again. */
            foreach (var other in _probes)
                /* Other serial probe which is still to be probed. */
                if (other is SerialProbe otherSerial && otherSerial.Result == ProbeResult.Planned)
                    if (otherSerial.Device.Type == probe.Device.Type && otherSerial.Device.Index == probe.Device.Index)
                        /* Can only have one device on a specific serial port. */
                        otherSerial.Result = ProbeResult.Skipped;
                    else if (otherSerial.Protocol == probe.Protocol)
                        /* Currently each protocol supports only one device - may in future change when ZIF sockets are supported per test position. */
                        otherSerial.Result = ProbeResult.Skipped;

            /* Update the configuration result. */
            switch (probe.Protocol)
            {
                case SerialProbeProtocols.FG30x:
                    _result.Configuration.FrequencyGenerator = probe.Device;
                    break;
                case SerialProbeProtocols.MT768:
                    _result.Configuration.MT768 = probe.Device;
                    break;
                case SerialProbeProtocols.ESxB:
                    _result.Configuration.ESxB = probe.Device;
                    break;
                case SerialProbeProtocols.PM8181:
                    _result.Configuration.PM8121ZIF = probe.Device;
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
