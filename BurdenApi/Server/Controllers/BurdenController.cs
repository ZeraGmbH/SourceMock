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
    /// Get the version of the current version.
    /// </summary>
    /// <returns>The version of the burden.</returns>
    [HttpGet("version"), SamAuthorize(WebSamRole.testcaseexecutor)]
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
    public Task<ActionResult> ProgramBurdenAsync(string? burden)
        => ActionResultMapper.SafeExecuteSerialPortCommandAsync(() => device.ProgramAsync(burden, logger));
}