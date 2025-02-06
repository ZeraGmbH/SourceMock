using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared.Provider;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Dosage;
using ZERA.WebSam.Shared.Models.Logging;
using MockDevices.Source;

namespace SourceApi.Controllers;

/// <summary>
/// Execute dosage functions on source.
/// </summary>
/// <remarks>
/// Initialize a new controller.
/// </remarks>
/// <param name="device">Serial port connected device to use.</param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/dosrestmock")]
public class DosageRestMockController([FromKeyedServices(DosageRestMockController.MockKey)] IDosage device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Start"), AllowAnonymous]
    [SwaggerOperation(OperationId = "Start")]
    public Task<ActionResult> StartDosageAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartDosageAsync(interfaceLogger));

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Cancel"), AllowAnonymous]
    [SwaggerOperation(OperationId = "Cancel")]
    public Task<ActionResult> CancelDosageAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.CancelDosageAsync(interfaceLogger));

    /// <summary>
    /// Change the DOS mode.
    /// </summary>
    /// <param name="on">Set to activate DOS mode - current will be turned on.</param>
    [HttpPost("DOSMode"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetDOSMode")]
    public Task<ActionResult> SetDOSModeAsync(bool on) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetDosageModeAsync(interfaceLogger, on));

    /// <summary>
    /// Read the current progress of a dosage operation.
    /// </summary>
    /// <returns>The information on the current dosage measurement.</returns>
    [HttpGet("Progress"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetProgress")]
    public Task<ActionResult<DosageProgress>> GetProgressAsync([ModelFromUri] MeterConstant meterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetDosageProgressAsync(interfaceLogger, meterConstant));

    /// <summary>
    /// Set the dosage energy.
    /// </summary>
    /// <param name="energy">The energy to sent to the DUT - in Wh.</param>
    /// <param name="meterConstant">The meter constant of the reference meter.</param>
    [HttpPut("Energy"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetEnergy")]
    public Task<ActionResult> SetEnergyAsync([ModelFromUri] ActiveEnergy energy, [ModelFromUri] MeterConstant meterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetDosageEnergyAsync(interfaceLogger, energy, meterConstant));

    /// <summary>
    /// Ask the server if the dosage is activated but the current is off.
    /// </summary>
    /// <returns>Dosage mode is on but current is off.</returns>
    [HttpGet("IsDosageCurrentOff"), AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "IsDosageCurrentOff")]
    public Task<ActionResult<bool>> IsDosageCurrentOffAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.CurrentSwitchedOffForDosageAsync(interfaceLogger));

    [HttpPost("NoSource"), AllowAnonymous]
    [SwaggerOperation(OperationId = "NoSource")]
    public async Task<ActionResult> NoSourceAsync()
    {
        if (device is IDosageMock dosageMock)
            await dosageMock.NoSourceAsync(interfaceLogger);

        return Ok();
    }
}
