using BarcodeApi.Actions;
using MeterTestSystemApi.Actions.Probing.HID;
using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing.Barcode;

/// <summary>
/// Probe for a barcode reader HID device.
/// </summary>
public class HIDBarcodeProbing : IHidProbeExecutor
{
    /// <inheritdoc/>
    public async Task<ProbeInfo> ExecuteAsync(HIDProbe probe, IInputDeviceManager devices)
    {
        var handler = $"event{probe.Index}";
        var all = await devices.GetHIDBarcodeCandidateDevices(0);
        var device = all.Find(a => a.GetList("Handlers")?.Contains(handler) == true);

        return new() { Message = device?.GetProperty("Name"), Succeeded = device != null };
    }
}