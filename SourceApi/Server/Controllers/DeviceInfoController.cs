using Microsoft.AspNetCore.Mvc;

using SourceApi.Actions.Source;

namespace SourceApi.Controllers;

/// <summary>
/// Request device dependant information.
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class DeviceInfoController : ControllerBase
{
    private readonly ISource _device;

    /// <summary>
    /// Initialize a new controller for the current request.
    /// </summary>
    /// <param name="device">The current device to use.</param>
    public DeviceInfoController(ISource device)
    {
        _device = device;
    }
}
