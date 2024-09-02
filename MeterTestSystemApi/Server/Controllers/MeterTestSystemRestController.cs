using MeterTestSystemApi.Actions.Device;
using MeterTestSystemApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/mtsrestmock")]
public class MeterTestSystemRestController([FromKeyedServices(MeterTestSystemRestController.MockKey)] IMeterTestSystem device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// Report the capabilities of the current meter test system.
    /// </summary>
    /// <returns>May be null if the meter test system does not allow configuration.</returns>
    [HttpGet, AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeterTestSystemCapabilities")]
    public Task<ActionResult<MeterTestSystemCapabilities>> GetCapabilitiesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetCapabilitiesAsync(interfaceLogger));

    /// <summary>
    /// Set the physical configuration of a meter test system.
    /// </summary>
    [HttpPut("AmplifiersAndReferenceMeter"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetAmplifiersAndReferenceMeter")]
    public Task<ActionResult> SetAmplifiersAndReferenceMeterAsync([FromBody] AmplifiersAndReferenceMeter request) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetAmplifiersAndReferenceMeterAsync(interfaceLogger, request));

    /// <summary>
    /// Report the current pysical configuration of the meter test system.
    /// </summary>
    /// <returns></returns>
    [HttpGet("AmplifiersAndReferenceMeter"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetAmplifiersAndReferenceMeter")]
    public Task<ActionResult<AmplifiersAndReferenceMeter>> GetAmplifiersAndReferenceMeterAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetAmplifiersAndReferenceMeterAsync(interfaceLogger));

    /// <summary>
    /// Read the firmware from the metering system.
    /// </summary>
    /// <returns>Firmware version of the metering system.</returns>
    [HttpGet("FirmwareVersion"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetFirmwareVersion")]
    public Task<ActionResult<MeterTestSystemFirmwareVersion>> GetFirmwareVersionAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetFirmwareVersionAsync(interfaceLogger));

    /// <summary>
    /// Read the current error conditions of the meter test system.
    /// </summary>
    /// <returns>All current error conditions.</returns>
    [HttpGet("ErrorConditions"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetErrorConditions")]
    public Task<ActionResult<ErrorConditions>> GetErrorConditionsAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetErrorConditionsAsync(interfaceLogger));
}
