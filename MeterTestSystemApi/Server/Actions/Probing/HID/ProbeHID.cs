using BarcodeApi.Actions;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Actions.Probing.HID;

/// <summary>
/// Run probe request against a HID device.
/// </summary>
public class ProbeHID(IServiceProvider services, IInputDeviceManager devices) : ProbeExecutor<HIDProbe>
{
    /// <inheritdoc/>
    protected override Task<ProbeInfo> OnExecuteAsync(HIDProbe probe)
    {
        /* Create the algorithm. */
        var algorithm = services.GetRequiredKeyedService<IHidProbeExecutor>(probe.Protocol);

        /* Create a new serial port connection accordung to the configuration. */
        return algorithm.ExecuteAsync(probe, devices);
    }
}