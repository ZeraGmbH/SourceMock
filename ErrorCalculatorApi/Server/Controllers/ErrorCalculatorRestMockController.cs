using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

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
    public Task<ActionResult> SetParametersAsync([ModelFromUri] MeterConstant dutMeterConstant, [ModelFromUri] Impulses impulses, [ModelFromUri] MeterConstant refMeterMeterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetErrorMeasurementParametersAsync(interfaceLogger, dutMeterConstant, impulses, refMeterMeterConstant));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Version"), AllowAnonymous]
    public Task<ActionResult<ErrorCalculatorFirmwareVersion>> GetFirmwareAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetFirmwareVersionAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatusAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetErrorStatusAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartSingle"), AllowAnonymous]
    public Task<ActionResult> StartSingleAsync(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurementAsync(interfaceLogger, false, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartContinuous"), AllowAnonymous]
    public Task<ActionResult> StartContinuousAsync(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurementAsync(interfaceLogger, true, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetSupportedMeterConnections"), AllowAnonymous]
    public Task<ActionResult<ErrorCalculatorMeterConnections[]>> GetSupportedMeterConnectionsAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetSupportedMeterConnectionsAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("Abort"), AllowAnonymous]
    public Task<ActionResult> AbortAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortErrorMeasurementAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("AbortAll"), AllowAnonymous]
    public Task<ActionResult> AbortAllAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortAllJobsAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("ActivateSource"), AllowAnonymous]
    public Task<ActionResult> ActivateSourceAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSourceAsync(interfaceLogger, true));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("DeactivateSource"), AllowAnonymous]
    public Task<ActionResult> DeactivateSourceAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSourceAsync(interfaceLogger, false));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Available"), AllowAnonymous]
    public async Task<ActionResult<bool>> IsAvailableAsync() =>
        Ok(await device.GetAvailableAsync(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("DutImpulses"), AllowAnonymous]
    public Task<ActionResult<Impulses?>> GetDeviceUnderTestImpulsesAsync() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetNumberOfDeviceUnderTestImpulsesAsync(interfaceLogger));

}
