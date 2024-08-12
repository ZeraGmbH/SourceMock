using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.Device;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Security;

namespace RefMeterApi.Controllers;

/// <summary>
/// Control a reference meter.
/// </summary>
/// <remarks>
/// Initialize a new controller.
/// </remarks>
/// <param name="device">Serial port connected device to use.</param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[TypeFilter<RefMeterApiExceptionFilter>]
[SamAuthorize(WebSamRole.testcaseexecutor)]
public class RefMeterController(IRefMeter device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// The current reference meter to use during the request.
    /// </summary>
    private readonly IRefMeter _device = device;

    /// <summary>
    /// Get the current measurement data.
    /// </summary>
    /// <param name="firstActiveCurrentPhase">Optional index of the first active current phase.</param>
    /// <returns>The current data.</returns>
    [HttpGet("ActualValues"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetActualValues")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasuredLoadpoint>> GetActualValues(int firstActiveCurrentPhase = -1) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.GetActualValues(interfaceLogger, firstActiveCurrentPhase));

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("MeasurementModes"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModes() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.GetMeasurementModes(interfaceLogger));

    /// <summary>
    /// Get the current measurement mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    [HttpGet("MeasurementMode"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementMode() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.GetActualMeasurementMode(interfaceLogger));

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode")]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetActualMeasurementMode(MeasurementModes mode) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.SetActualMeasurementMode(interfaceLogger, mode));

    /// <summary>
    /// Report if the reference meter can be used.
    /// </summary>
    [HttpGet("Available"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "ReferenceMeterIsAvailable")]
    public ActionResult<bool> IsAvailable() =>
        Ok(_device.GetAvailable(interfaceLogger));

    /// <summary>
    /// Get the current meter constant of the reference meter.
    /// </summary>
    /// <returns>The meter constant (impulses per kWh).</returns>
    [HttpGet("MeterConstant"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetMeterConstant")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<MeterConstant>> GetMeterConstant()
        => ActionResultMapper.SafeExecuteSerialPortCommand(() => _device.GetMeterConstant(interfaceLogger));
}
