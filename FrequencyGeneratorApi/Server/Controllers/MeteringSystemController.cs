using MeteringSystemApi.Actions.Device;
using MeteringSystemApi.Model;
using MeteringSystemApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MeteringSystemApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class MeteringSystemController : ControllerBase
{
    private readonly IMeteringSystem _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    public MeteringSystemController(IMeteringSystem device)
    {
        _device = device;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetMeteringSystemCapabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeteringSystemCapabilities>> GetCapabilities() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetCapabilities);

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

    /// <summary>
    /// Read the firmware from the metering system.
    /// </summary>
    /// <returns>Firmware version of the metering system.</returns>
    [HttpGet("FirmwareVersion")]
    [SwaggerOperation(OperationId = "GetFirmwareVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeteringSystemFirmwareVersion>> GetFirmwareVersion() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetFirmwareVersion);
}
