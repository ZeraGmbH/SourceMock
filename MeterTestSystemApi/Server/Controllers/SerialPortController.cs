using MeterTestSystemApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;

namespace MeterTestSystemApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class SerialPortController(ICustomSerialPortFactory ports, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="requests"></param>
    /// <returns></returns>
    [HttpPost("{name}"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "ExecuteSerialPortRequests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<SerialPortReply[]>> ExecuteSerialPortRequestsAsync(string name, [FromBody] SerialPortCommand[] requests)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => ports.ExecuteAsync(name, requests, interfaceLogger));
}