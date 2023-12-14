using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MeterTestSystemApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class MeterTestSystemController : ControllerBase
{
    private readonly IMeterTestSystem _device;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="device"></param>
    public MeterTestSystemController(IMeterTestSystem device)
    {
        _device = device;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetMeterTestSystemCapabilities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeterTestSystemCapabilities>> GetCapabilities() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetCapabilities);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPut("AmplifiersAndReferenceMeters")]
    [SwaggerOperation(OperationId = "SetAmplifiersAndReferenceMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetAmplifiersAndReferenceMeter([FromBody] AmplifiersAndReferenceMeters request) =>
        Utils.SafeExecuteSerialPortCommand(() => _device.SetAmplifiersAndReferenceMeter(request));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("AmplifiersAndReferenceMeters")]
    [SwaggerOperation(OperationId = "GetAmplifiersAndReferenceMeter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<AmplifiersAndReferenceMeters> GetAmplifiersAndReferenceMeter() =>
        _device.AmplifiersAndReferenceMeters;

    /// <summary>
    /// Read the firmware from the metering system.
    /// </summary>
    /// <returns>Firmware version of the metering system.</returns>
    [HttpGet("FirmwareVersion")]
    [SwaggerOperation(OperationId = "GetFirmwareVersion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeterTestSystemFirmwareVersion>> GetFirmwareVersion() =>
        Utils.SafeExecuteSerialPortCommand(_device.GetFirmwareVersion);
}
