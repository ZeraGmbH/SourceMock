using Microsoft.AspNetCore.Mvc;

using WebSamDeviceApis.Actions.Device;
using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Controllers;

/// <summary>
/// Request device dependant information.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DeviceInfoController : ControllerBase
{
    private readonly IDevice _device;

    /// <summary>
    /// Initialize a new controller for the current request.
    /// </summary>
    /// <param name="device">The current device to use.</param>
    public DeviceInfoController(IDevice device)
    {
        _device = device;
    }

    /// <summary>
    /// Read the firmware from the device.
    /// </summary>
    /// <returns>Firmware version of the device.</returns>
    [HttpGet("GetFirmwareVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<DeviceFirmwareVersion> GetFirmwareVersion()
    {
        return _device.GetFirmwareVersion();
    }
}
