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
    public Task<ActionResult> SetParameters([ModelFromUri] MeterConstant dutMeterConstant, [ModelFromUri] Impulses impulses, [ModelFromUri] MeterConstant refMeterMeterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetErrorMeasurementParameters(interfaceLogger, dutMeterConstant, impulses, refMeterMeterConstant));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Version"), AllowAnonymous]
    public Task<ActionResult<ErrorCalculatorFirmwareVersion>> GetFirmware() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetFirmwareVersion(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatus() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetErrorStatus(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartSingle"), AllowAnonymous]
    public Task<ActionResult> StartSingle(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurement(interfaceLogger, false, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartContinuous"), AllowAnonymous]
    public Task<ActionResult> StartContinuous(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.StartErrorMeasurement(interfaceLogger, true, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetSupportedMeterConnections"), AllowAnonymous]
    public Task<ActionResult<ErrorCalculatorMeterConnections[]>> GetSupportedMeterConnections() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetSupportedMeterConnections(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("Abort"), AllowAnonymous]
    public Task<ActionResult> Abort() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortErrorMeasurement(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("AbortAll"), AllowAnonymous]
    public Task<ActionResult> AbortAll() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.AbortAllJobs(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("ActivateSource"), AllowAnonymous]
    public Task<ActionResult> ActivateSource() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSource(interfaceLogger, true));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("DeactivateSource"), AllowAnonymous]
    public Task<ActionResult> DeactivateSource() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ActivateSource(interfaceLogger, false));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Available"), AllowAnonymous]
    public ActionResult<bool> IsAvailable() =>
        Ok(device.GetAvailable(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("DutImpulses"), AllowAnonymous]
    public Task<ActionResult<Impulses?>> GetDeviceUnderTestImpulses() =>
        ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetNumberOfDeviceUnderTestImpulses(interfaceLogger));

}
