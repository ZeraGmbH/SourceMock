using ZERA.WebSam.Shared.Models.ErrorCalculator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Provider;

namespace ErrorCalculatorApi.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="device"></param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/ecrestmock")]
public class ErrorCalculatorRestMockController([FromKeyedServices(ErrorCalculatorRestMockController.MockKey)] IErrorCalculator device, IInterfaceLogger interfaceLogger) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dutMeterConstant"></param>
    /// <param name="impulses"></param>
    /// <param name="refMeterMeterConstant"></param>
    [HttpPut, AllowAnonymous]
    [SwaggerOperation(OperationId = "SetParameters")]
    public Task<ActionResult> SetParametersAsync([ModelFromUri] MeterConstant dutMeterConstant, [ModelFromUri] Impulses impulses, [ModelFromUri] MeterConstant refMeterMeterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetErrorMeasurementParametersAsync(interfaceLogger, dutMeterConstant, impulses, refMeterMeterConstant));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Version"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetFirmware")]
    public Task<ActionResult<ErrorCalculatorFirmwareVersion>> GetFirmwareAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetFirmwareVersionAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    [SwaggerOperation(OperationId = "GetStatus")]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatusAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetErrorStatusAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartSingle"), AllowAnonymous]
    [SwaggerOperation(OperationId = "StartSingle")]
    public Task<ActionResult> StartSingleAsync(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurementAsync(interfaceLogger, false, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartContinuous"), AllowAnonymous]
    [SwaggerOperation(OperationId = "StartContinuous")]
    public Task<ActionResult> StartContinuousAsync(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurementAsync(interfaceLogger, true, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetSupportedMeterConnections"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetSupportedMeterConnections")]
    public Task<ActionResult<ErrorCalculatorMeterConnections[]>> GetSupportedMeterConnectionsAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetSupportedMeterConnectionsAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("Abort"), AllowAnonymous]
    [SwaggerOperation(OperationId = "Abort")]
    public Task<ActionResult> AbortAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortErrorMeasurementAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("AbortAll"), AllowAnonymous]
    [SwaggerOperation(OperationId = "AbortAll")]
    public Task<ActionResult> AbortAllAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortAllJobsAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("ActivateSource"), AllowAnonymous]
    [SwaggerOperation(OperationId = "ActivateSource")]
    public Task<ActionResult> ActivateSourceAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSourceAsync(interfaceLogger, true));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("DeactivateSource"), AllowAnonymous]
    [SwaggerOperation(OperationId = "DeactivateSource")]
    public Task<ActionResult> DeactivateSourceAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSourceAsync(interfaceLogger, false));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Available"), AllowAnonymous]
    [SwaggerOperation(OperationId = "ErrorCalculatorIsAvailable")]
    public async Task<ActionResult<bool>> IsAvailableAsync() =>
        Ok(await device.GetAvailableAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("DutImpulses"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetDeviceUnderTestImpulses")]
    public Task<ActionResult<Impulses?>> GetDeviceUnderTestImpulsesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetNumberOfDeviceUnderTestImpulsesAsync(interfaceLogger));

}
