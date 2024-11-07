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
    public Task<ActionResult<MeasuredLoadpoint>> GetActualValuesAsync(int firstActiveCurrentPhase = -1) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetActualValuesAsync(interfaceLogger, firstActiveCurrentPhase));

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
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetMeasurementModesAsync(interfaceLogger));

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
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementModeAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetActualMeasurementModeAsync(interfaceLogger));

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetActualMeasurementModeAsync(MeasurementModes mode) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.SetActualMeasurementModeAsync(interfaceLogger, mode));

    /// <summary>
    /// Report if the reference meter can be used.
    /// </summary>
    [HttpGet("Available"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "ReferenceMeterIsAvailable")]
    public async Task<ActionResult<bool>> IsAvailableAsync() =>
        Ok(await _device.GetAvailableAsync(interfaceLogger));

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
    public Task<ActionResult<MeterConstant>> GetMeterConstantAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetMeterConstantAsync(interfaceLogger));

    /// <summary>
    /// Get information on the reference meter.
    /// </summary>
    /// <returns>Report various information on the reference meter.</returns>
    [HttpGet, SamAuthorize]
    [SwaggerOperation(OperationId = "GetMeterInformation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<ReferenceMeterInformation>> GetMeterInformationAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetMeterInformationAsync(interfaceLogger));

    
    
    /// <summary>
    /// Get voltage ranges on the reference meter.
    /// </summary>
    /// <returns>Voltage ranges.</returns>
    [HttpGet("VoltageRanges"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetVoltageRanges")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<Voltage[]>> GetVoltageRangesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetVoltageRangesAsync(interfaceLogger));

    /// <summary>
    /// Get current ranges on the reference meter.
    /// </summary>
    /// <returns>Current ranges.</returns>
    [HttpGet("CurrentRanges"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetCurrentRanges")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<Current[]>> GetCurrentRangesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetCurrentRangesAsync(interfaceLogger));

    /// <summary>
    /// Set voltage range on the reference meter.
    /// </summary>
    /// <param name="voltage">Upper limit of range.</param>
    [HttpPut("VoltageRange"), SamAuthorize]
    [SwaggerOperation(OperationId = "SetVoltageRange")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetVoltageRangeAsync(Voltage voltage) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.SetVoltageRangeAsync(interfaceLogger, voltage));

    /// <summary>
    /// Set current range on the reference meter.
    /// </summary>
    [HttpPut("CurrentRange"), SamAuthorize]
    [SwaggerOperation(OperationId = "SetCurrentRange")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetCurrentRangeAsync(Current current) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.SetCurrentRangeAsync(interfaceLogger, current));

    /// <summary>
    /// Switch between automatic and manual on the reference meter.
    /// </summary>
    /// <param name="voltageRanges"></param>
    /// <param name="currentRanges"></param>
    /// <param name="pll"></param>
    [HttpPut("Automatic"), SamAuthorize]
    [SwaggerOperation(OperationId = "SetAutomatic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SetAutomaticAsync(bool voltageRanges = true, bool currentRanges = true, bool pll = true) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.SetAutomaticAsync(interfaceLogger, voltageRanges, currentRanges, pll));

    /// <summary>
    /// Select pll channel - only possible if pll automatic is set to false
    /// </summary>
    /// <param name="pll"></param>
    [HttpPut("PllChannel"), SamAuthorize]
    [SwaggerOperation(OperationId = "SelectPllChannel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> SelectPllChannelAsync(PllChannel pll) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.SelectPllChannelAsync(interfaceLogger, pll));

    /// <summary>
    /// Select pll channel - only possible if pll automatic is set to false
    /// </summary>
    [HttpGet("RefMeterStatus"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetRefMeterStatus")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<RefMeterStatus>> GetRefMeterStatusAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => _device.GetRefMeterStatusAsync(interfaceLogger));
}
