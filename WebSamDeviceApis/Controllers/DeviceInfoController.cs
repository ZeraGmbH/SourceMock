using Microsoft.AspNetCore.Mvc;

using SerialPortProxy;

using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DeviceInfoController : ControllerBase
{
    private readonly SerialPortService _service;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="service"></param>
    public DeviceInfoController(SerialPortService service)
    {
        _service = service;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet("GetFirmwareVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public DeviceFirmwareVersion GetFirmwareVersion()
    {
        throw new NotImplementedException();
    }
}
