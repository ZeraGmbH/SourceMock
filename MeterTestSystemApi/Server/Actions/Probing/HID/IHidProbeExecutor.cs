using BarcodeApi.Actions;
using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing.HID;

/// <summary>
/// Interface for probing a HID device.
/// </summary>
public interface IHidProbeExecutor
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="device">Device to use.</param>
    /// <param name="devices">Cached list of devices.</param>
    /// <returns>Error message or empty.</returns>
    Task<ProbeInfo> ExecuteAsync(HIDProbe device, IInputDeviceManager devices);
}