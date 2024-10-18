using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;
using ZIFApi.Models;

namespace ZIFApi.Controller;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ZIFController(IZIFDevice[] devices, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Count"), SamAuthorize]
    [SwaggerOperation(OperationId = "NumberOfZIFSockets")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<int> Count() => Ok(devices.Length);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    [HttpGet("Available/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "IsZIFSocketAvailable")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<bool> Available(int pos = 0) => Ok(devices[pos] != null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    [HttpGet("Version/{pos?}"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetZIFSocketVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ZIFVersionInfo>> GetVersion(int pos = 0)
        => Ok(await devices[pos].GetVersion(interfaceLogger));
}