using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MeterTestSystemApi.Services.Probing;

partial class ConfigurationProbePlan
{
    private void AddHIDProbes()
    {
        for (uint i = 0; i < _request.HIDEventCount; i++)
            if (_request.Configuration.BarcodeReader.HasValue)
                _probes.Add(new HIDProbe()
                {
                    Protocol = HIDProbeProtocols.Barcode,
                    Index = _request.MinHIDEvent + i
                });
    }


    private async Task ProbeHidAsync(HIDProbe probe)
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

            _logger.LogInformation("HID device {Probe} detected: {Message}", probe.ToString(), info.Message);

            /* Do not inspect this device or this target type again. */
            foreach (var other in _probes)
                if (other is HIDProbe otherHid && otherHid.Result == ProbeResult.Planned)
                    if (otherHid.Index == probe.Index)
                        otherHid.Result = ProbeResult.Skipped;
                    else if (otherHid.Protocol == probe.Protocol)
                        otherHid.Result = ProbeResult.Skipped;

            /* Update the configuration result - keyboard means barcode. */
            switch (probe.Protocol)
            {
                case HIDProbeProtocols.Barcode:
                    _result.Configuration.BarcodeReader = probe.Index;
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
