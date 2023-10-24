using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

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
    [HttpGet("FirmwareVersion")]
    [SwaggerOperation(OperationId = "GetFirmwareVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeviceFirmwareVersion>> GetFirmwareVersion()
    {
        try
        {
            return Ok(await _device.GetFirmwareVersion());
        }
        catch (TimeoutException)
        {
            return Problem(
                detail: "Source operation timed out.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
        catch (InvalidOperationException e)
        {
            return Problem(
                detail: $"Unable to execute request: {e.Message}.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
        catch (OperationCanceledException e)
        {
            return Problem(
                detail: $"Execution has been cancelled: {e.Message}.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}
