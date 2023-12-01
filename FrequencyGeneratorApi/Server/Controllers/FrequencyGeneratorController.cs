using FrequencyGeneratorApi.Actions.Device;
using FrequencyGeneratorApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FrequencyGeneratorApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class FrequencyGeneratorController : ControllerBase
{
    private readonly IFrequencyGenerator _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    public FrequencyGeneratorController(IFrequencyGenerator device)
    {
        _device = device;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetFrequencyGeneratorCapabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<FrequencyGeneratorCapabilities>> GetCapabilities() =>
        Utils.SafeExecuteSerialPortCommand(() => _device.GetCapabilities());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [SwaggerOperation(OperationId = "SetAmplifiersAndReferenceMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetAmplifiersAndReferenceMeter([FromBody] SetAmplifiersAndReferenceMeterRequest request) =>
        Utils.SafeExecuteSerialPortCommand(() => _device.SetAmplifiersAndReferenceMeter(request.VoltageAmplifier, request.CurrentAmplifier, request.ReferenceMeter));
}
