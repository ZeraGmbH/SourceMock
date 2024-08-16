using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [HttpPut]
    public Task<ActionResult> SetParameters([ModelFromUri] MeterConstant dutMeterConstant, [ModelFromUri] Impulses impulses, [ModelFromUri] MeterConstant refMeterMeterConstant) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.SetErrorMeasurementParameters(interfaceLogger, dutMeterConstant, impulses, refMeterMeterConstant));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Version")]
    public Task<ActionResult<ErrorCalculatorFirmwareVersion>> GetFirmware() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.GetFirmwareVersion(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Task<ActionResult<ErrorMeasurementStatus>> GetStatus() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.GetErrorStatus(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartSingle")]
    public Task<ActionResult> StartSingle(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.StartErrorMeasurement(interfaceLogger, false, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    [HttpPost("StartContinuous")]
    public Task<ActionResult> StartContinuous(ErrorCalculatorMeterConnections? connection) =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.StartErrorMeasurement(interfaceLogger, true, connection));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetSupportedMeterConnections")]
    public Task<ActionResult<ErrorCalculatorMeterConnections[]>> GetSupportedMeterConnections() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.GetSupportedMeterConnections(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("Abort")]
    public Task<ActionResult> Abort() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.AbortErrorMeasurement(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("AbortAll")]
    public Task<ActionResult> AbortAll() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.AbortAllJobs(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("ActivateSource")]
    public Task<ActionResult> ActivateSource() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.ActivateSource(interfaceLogger, true));

    /// <summary>
    /// 
    /// </summary>
    [HttpPost("DeactivateSource")]
    public Task<ActionResult> DeactivateSource() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.ActivateSource(interfaceLogger, false));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("Available")]
    public ActionResult<bool> IsAvailable() =>
        Ok(device.GetAvailable(interfaceLogger));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("DutImpulses")]
    public Task<ActionResult<Impulses?>> GetDeviceUnderTestImpulses() =>
        ActionResultMapper.SafeExecuteSerialPortCommand(() => device.GetNumberOfDeviceUnderTestImpulses(interfaceLogger));

}
