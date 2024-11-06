using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;

namespace BurdenApi.Controllers;

/// <summary>
/// Access the burden.
/// </summary>
/// <param name="burden">Burden to use.</param>
/// <param name="logger">Logging helper.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BurdenController(IBurden burden, IInterfaceLogger logger) : ControllerBase
{
    /// <summary>
    /// Get the version of the current version.
    /// </summary>
    /// <returns>The version of the burden.</returns>
    [HttpGet, SamAuthorize]
    [SwaggerOperation(OperationId = "GetBurdenVersion")]
    public Task<ActionResult<BurdenVersion>> GetVersionAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => burden.GetVersionAsync(logger));
}