using System.Text.RegularExpressions;
using MeterTestSystemApi.Models;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
public class SerialPortController(IServiceProvider services, IInterfaceLogger interfaceLogger) : ControllerBase
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
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => Task.Run(async () =>
            {
                var port = services.GetRequiredKeyedService<ICustomSerialPortConnection>(name);
                var exec = port.CreateExecutor(InterfaceLogSourceTypes.SerialPort, name);

                var commands =
                    requests
                        .Select(r => r.UseRegularExpression ? SerialPortRequest.Create(r.Command, new Regex(r.Reply)) : SerialPortRequest.Create(r.Command, r.Reply))
                        .ToArray();

                await Task.WhenAll(exec.Execute(interfaceLogger, commands));

                return
                    commands
                        .Select(c =>
                        {
                            var reply = new SerialPortReply();
                            var match = c.EndMatch;

                            if (match == null)
                                reply.Matches.Add(c.Command);
                            else
                                reply.Matches.AddRange(match.Groups.Values.Select(g => g.Value));

                            return reply;
                        })
                        .ToArray();
            }));
}