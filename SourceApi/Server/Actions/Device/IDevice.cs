using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Device;

/// <summary>
/// Describes any device - currently used for moving tests.
/// </summary>
public interface IDevice
{
    /// <summary>
    /// Retrieve information on the firmware version.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<DeviceFirmwareVersion> GetFirmwareVersion();
}
