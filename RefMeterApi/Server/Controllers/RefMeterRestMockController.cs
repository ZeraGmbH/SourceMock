using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Controllers;

/// <summary>
/// 
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/rmrestmock")]
public class RefMeterRestMockController([FromKeyedServices(RefMeterRestMockController.MockKey)] IRefMeter device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// Get the current measurement data.
    /// </summary>
    /// <param name="firstActiveCurrentPhase">Optional index of the first active current phase.</param>
    /// <returns>The current data.</returns>
    [HttpGet("ActualValues"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetActualValues")]
    public Task<ActionResult<MeasuredLoadpoint>> GetActualValues(int firstActiveCurrentPhase = -1) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetActualValues(interfaceLogger, firstActiveCurrentPhase));

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("MeasurementModes"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModes() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeasurementModes(interfaceLogger));

    /// <summary>
    /// Get the current measurement mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    [HttpGet("MeasurementMode"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeasurementMode")]
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementMode() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetActualMeasurementMode(interfaceLogger));

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    public Task<ActionResult> SetActualMeasurementMode(MeasurementModes mode) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetActualMeasurementMode(interfaceLogger, mode));

    /// <summary>
    /// Report if the reference meter can be used.
    /// </summary>
    [HttpGet("Available"), AllowAnonymous]
    [SwaggerOperation(OperationId = "ReferenceMeterIsAvailable")]
    public ActionResult<bool> IsAvailable() =>
        Ok(device.GetAvailable(interfaceLogger));

    /// <summary>
    /// Get the current meter constant of the reference meter.
    /// </summary>
    /// <returns>The meter constant (impulses per kWh).</returns>
    [HttpGet("MeterConstant"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeterConstant")]
    public Task<ActionResult<MeterConstant>> GetMeterConstant()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeterConstant(interfaceLogger));

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("Version"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeterInformation")]
    public Task<ActionResult<ReferenceMeterInformation>> GetMeterInformation() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeterInformation(interfaceLogger));

}