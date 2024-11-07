using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
using SerialPortProxy;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;

namespace BurdenApi.Controllers;

/// <summary>
/// Access the burden.
/// </summary>
/// <param name="device">Burden to use.</param>
/// <param name="logger">Logging helper.</param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class BurdenController(IBurden device, IInterfaceLogger logger) : ControllerBase
{
    /// <summary>
    /// Check if a burden is configured.
    /// </summary>
    /// <returns>Set if burden is available.</returns>
    [HttpGet, SamAuthorize]
    [SwaggerOperation(OperationId = "BurdenAvailable")]
    public ActionResult<bool> HasBurden() => device.IsAvailable;

    /// <summary>
    /// Get the version of the current version.
    /// </summary>
    /// <returns>The version of the burden.</returns>
    [HttpGet("version"), SamAuthorize]
    [SwaggerOperation(OperationId = "GetBurdenVersion")]
    public Task<ActionResult<BurdenVersion>> GetVersionAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetVersionAsync(logger));

    /// <summary>
    /// Retrieve a single calibration.
    /// </summary>
    /// <param name="burden">Name of the burden.</param>
    /// <param name="range">Range to use.</param>
    /// <param name="step">Step to request.</param>
    /// <returns>null if step is not calibrated.</returns>
    [HttpGet("calibration"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "GetBurdenStepCalibration")]
    public Task<ActionResult<Calibration?>> GetCalibratioAsync(string burden, string range, string step)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetCalibrationAsync(burden, range, step, logger));

    /// <summary>
    /// Activate the burden.
    /// </summary>
    [HttpPost("on"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "ActivateBurden")]
    public Task<ActionResult> ActivateAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetActiveAsync(true, logger));

    /// <summary>
    /// Activate the burden.
    /// </summary>
    [HttpPost("off"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "DeactivateBurden")]
    public Task<ActionResult> DeactivateAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetActiveAsync(false, logger));

    /// <summary>
    /// Request all known burdens.
    /// </summary>
    /// <returns>List of burdens.</returns>
    [HttpGet("burdens"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "GetBurdens")]
    public Task<ActionResult<string[]>> GetBurdensAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetBurdensAsync(logger));

    /// <summary>
    /// Program burdens.
    /// </summary>
    /// <remarks>
    /// May take a very liong time so change the client
    /// timeout accordingly.
    /// </remarks>
    [HttpPost("program"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "ProgramBurden")]
    public Task<ActionResult> ProgramAsync(string? burden)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ProgramAsync(burden, logger));

    /// <summary>
    /// Retrieve the status of the burden.
    /// </summary>
    /// <returns>Status information.</returns>
    [HttpGet("status"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "GetBurdenStatus")]
    public Task<ActionResult<BurdenStatus>> GetStatusAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.GetStatusAsync(logger));

    /// <summary>
    /// Get the current measurement values
    /// </summary>
    /// <returns>Current values for the single phase.</returns>
    [HttpGet("values"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "MeasureBurden")]
    public Task<ActionResult<BurdenValues>> MeasureAsync()
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.MeasureAsync(logger));

    /// <summary>
    /// Set the active burden.
    /// </summary>
    /// <param name="burden">Burden to make the active one.</param>
    [HttpPut("burden"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetActiveBurden")]
    public Task<ActionResult> SetActiveBurdenAsync(string burden)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetBurdenAsync(burden, logger));

    /// <summary>
    /// Set the active range.
    /// </summary>
    /// <param name="range">Range to use.</param>
    [HttpPut("range"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetActiveBurdenRange")]
    public Task<ActionResult> SetActiveRangeAsync(string range)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetRangeAsync(range, logger));

    /// <summary>
    /// Set the active step.
    /// </summary>
    /// <param name="step">Step to use.</param>
    /// <returns></returns>
    [HttpPut("step"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetActiveBurdenStep")]
    public Task<ActionResult> SetActiveStepAsync(string step)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetStepAsync(step, logger));

    /// <summary>
    /// Switch the current calibration on the FETs.
    /// </summary>
    /// <param name="calibration">New calibration</param>
    [HttpPut("fet"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetBurdenCalibrationTransient")]
    public Task<ActionResult> SetTransientAsync([FromBody] Calibration calibration)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetTransientCalibrationAsync(calibration, logger));

    /// <summary>
    /// Update the calibration for a single step.
    /// </summary>
    /// <param name="burden">Burden to use.</param>
    /// <param name="range">Range to change.</param>
    /// <param name="step">Step to change.</param>
    /// <param name="calibration">New calibration data.</param>
    [HttpPut("calibration"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [SwaggerOperation(OperationId = "SetBurdenCalibration")]
    public Task<ActionResult> SetPermanentAsync(string burden, string range, string step, [FromBody] Calibration calibration)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.SetPermanentCalibrationAsync(burden, range, step, calibration, logger));
}