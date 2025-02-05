using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Provider;

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
    public Task<ActionResult<MeasuredLoadpoint>> GetActualValuesAsync(int firstActiveCurrentPhase = -1) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetActualValuesAsync(interfaceLogger, firstActiveCurrentPhase));

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("MeasurementModes"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeasurementModes")]
    public Task<ActionResult<MeasurementModes[]>> GetMeasurementModesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeasurementModesAsync(interfaceLogger));

    /// <summary>
    /// Get the current measurement mode.
    /// </summary>
    /// <returns>The current mode.</returns>
    [HttpGet("MeasurementMode"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeasurementMode")]
    public Task<ActionResult<MeasurementModes?>> GetActualMeasurementModeAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetActualMeasurementModeAsync(interfaceLogger));

    /// <summary>
    /// Set the current measurement mode.
    /// </summary>
    [HttpPut("MeasurementMode"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetMeasurementMode")]
    public Task<ActionResult> SetActualMeasurementModeAsync(MeasurementModes mode) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetActualMeasurementModeAsync(interfaceLogger, mode));

    /// <summary>
    /// Report if the reference meter can be used.
    /// </summary>
    [HttpGet("Available"), AllowAnonymous]
    [SwaggerOperation(OperationId = "ReferenceMeterIsAvailable")]
    public async Task<ActionResult<bool>> IsAvailableAsync() =>
        Ok(await device.GetAvailableAsync(interfaceLogger));

    /// <summary>
    /// Get the current meter constant of the reference meter.
    /// </summary>
    /// <returns>The meter constant (impulses per kWh).</returns>
    [HttpGet("MeterConstant"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeterConstant")]
    public Task<ActionResult<MeterConstant>> GetMeterConstantAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeterConstantAsync(interfaceLogger));

    /// <summary>
    /// Get the list of supported measurement modes.
    /// </summary>
    /// <returns>The list of modes.</returns>
    [HttpGet("Version"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetMeterInformation")]
    public Task<ActionResult<ReferenceMeterInformation>> GetMeterInformationAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetMeterInformationAsync(interfaceLogger));

}