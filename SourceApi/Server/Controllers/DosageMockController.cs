using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using SourceApi.Actions.Source;
using SourceApi.Model;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;

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
public class DosageMockController([FromKeyedServices(DosageMockController.MockKey)] ISource device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Start"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "Start")]
    public Task<ActionResult> StartDosage() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.StartDosage(interfaceLogger));

    /// <summary>
    /// Start a dosage meaurement.
    /// </summary>
    [HttpPost("Cancel"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "Cancel")]
    public Task<ActionResult> CancelDosage() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.CancelDosage(interfaceLogger));

    /// <summary>
    /// Change the DOS mode.
    /// </summary>
    /// <param name="on">Set to activate DOS mode - current will be turned on.</param>
    [HttpPost("DOSMode"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetDOSMode")]
    public Task<ActionResult> SetDOSMode(bool on) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.SetDosageMode(interfaceLogger, on));

    /// <summary>
    /// Read the current progress of a dosage operation.
    /// </summary>
    /// <returns>The information on the current dosage measurement.</returns>
    [HttpGet("Progress"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "GetProgress")]
    public Task<ActionResult<DosageProgress>> GetProgress([ModelFromUri] MeterConstant meterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.GetDosageProgress(interfaceLogger, meterConstant));

    /// <summary>
    /// Set the dosage energy.
    /// </summary>
    /// <param name="energy">The energy to sent to the DUT - in Wh.</param>
    /// <param name="meterConstant">The meter constant of the reference meter.</param>
    [HttpPut("Energy"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetEnergy")]
    public Task<ActionResult> SetEnergy([ModelFromUri] ActiveEnergy energy, [ModelFromUri] MeterConstant meterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.SetDosageEnergy(interfaceLogger, energy, meterConstant));

    /// <summary>
    /// Ask the server if the dosage is activated but the current is off.
    /// </summary>
    /// <returns>Dosage mode is on but current is off.</returns>
    [HttpGet("IsDosageCurrentOff"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "IsDosageCurrentOff")]
    public Task<ActionResult<bool>> IsDosageCurrentOff() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.CurrentSwitchedOffForDosage(interfaceLogger));
}
