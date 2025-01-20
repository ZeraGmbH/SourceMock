using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Security;
using ZERA.WebSam.Shared;

namespace SourceApi.Controllers;

/// <summary>
/// Controls a source.
/// </summary>
/// <remarks>
/// Constructor for a SouceController.
/// </remarks>
/// <param name="logger">Injected logger.</param>
/// <param name="source">Injected source.</param>
/// <param name="sourceHealth"></param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class SourceController(
    ILogger<SourceController> logger,
    ISource source,
    ISourceHealthUtils sourceHealth,
    IInterfaceLogger interfaceLogger
) : Controller
{
    /// <summary>
    /// Gets the capabilities of this source.
    /// </summary>
    /// <returns>The corresponding<see cref="SourceCapabilities"/>-Object for this source.</returns>
    /// /// <response code="200">If the capabilities could be returned successfully.</response>
    [HttpGet("Capabilities"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "GetCapabilities")]
    public async Task<ActionResult<SourceCapabilities>> GetCapablitiesAsync() => Ok(await source.GetCapabilitiesAsync(interfaceLogger));

    /// <summary>
    /// Gets the capabilities of this source.
    /// </summary>
    /// <returns>The corresponding<see cref="SourceCapabilities"/>-Object for this source.</returns>
    /// <response code="200">If the capabilities could be returned successfully.</response>
    [HttpGet("Available"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "SourceIsAvailable")]
    public async Task<ActionResult<bool>> IsAvailableAsync() => Ok(await source.GetAvailableAsync(interfaceLogger));

    /// <summary>
    /// Sets a loadpoint without turning on the source.
    /// </summary>
    /// <param name="loadpoint">The loadpoint to be set.</param>
    /// <returns>The result of the operation, see responses.</returns>
    /// <response code="200">If the loadpoint could be set correctly.</response>
    /// <response code="400">If the loadpoint was (generally) malformed. The loadpoint cannot be set with any source.</response>
    /// <response code="422">If the loadpoint was wellformed but invalid. The loadpoint cannot be set with this source.</response>
    /// <response code="500">If an unexpected error occured.</response>
    [HttpPut("Loadpoint"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(OperationId = "SetLoadpoint")]
    [ActiveOperation(ForbidScript = true)]
    public async Task<ActionResult> SetLoadpointAsync([FromBody] TargetLoadpoint loadpoint)
    {
        logger.LogTrace($"Loadpoint to be set: {loadpoint}");

        var srcResult = await sourceHealth.SetLoadpointAsync(interfaceLogger, loadpoint);

        switch (srcResult)
        {
            case SourceApiErrorCodes.SUCCESS:
                logger.LogTrace("Loadpoint was successfully set.");

                return Ok();
            case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES:
            case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID:
            case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID:
            case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS:
            case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_FREQUENCY_INVALID:
            case SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID:
            case SourceApiErrorCodes.SUCCESS_NOT_ACTIVATED:
                logger.LogInformation(srcResult.ToString());

                return Problem(
                    detail: srcResult.ToUserFriendlyString(),
                    statusCode: StatusCodes.Status422UnprocessableEntity);
            default:
                logger.LogError($"Unkown response from source: {srcResult}");

                return Problem(
                    detail: $"Unkown Response from source: {srcResult}, {srcResult.ToUserFriendlyString()}",
                    statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Turns off the source.
    /// </summary>
    /// <returns>The result of the operation, see responses.</returns>
    /// <response code="200">If the source was turned off successfully.</response>
    /// <response code="409">If the source could not be turned off successfully.</response>
    /// <response code="500">If an unexpected error occured.</response>
    [HttpPost("TurnOff"), SamAuthorize(WebSamRole.testcaseexecutor)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(OperationId = "TurnOff")]
    public async Task<ActionResult> TurnOffAsync()
    {
        var srcResult = await sourceHealth.TurnOffAsync(interfaceLogger);

        switch (srcResult)
        {
            case SourceApiErrorCodes.SUCCESS:
                logger.LogTrace("Source was turned off.");
                return Ok();
            default:
                logger.LogError($"Unkown response from source: {srcResult}");
                return Problem(
                    detail: "Unkown Response from source.",
                    statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Gets the currently active loadpoint.
    /// </summary>
    /// <returns>The loadpoint. HTTP 204 when the source is turned off.</returns>
    /// <response code="200">If there is a loadpoint to be returned.</response>
    /// <response code="204">If the source is turned off.</response>
    [HttpGet("Loadpoint"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [SwaggerOperation(OperationId = "GetLoadpoint")]
    public async Task<ActionResult<TargetLoadpoint>> GetCurrentLoadpointAsync()
    {
        var loadpoint = await source.GetCurrentLoadpointAsync(interfaceLogger);

        return loadpoint == null
            ? NoContent()
            : Ok(loadpoint);
    }

    /// <summary>
    /// Report information on the last loadpoint activated.
    /// </summary>
    /// <returns>The information including some dates.</returns>
    /// <response code="200">If there is a loadpointinfo to be returned.</response>
    [HttpGet("LoadpointInfo"), SamAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(OperationId = "GetLoadpointInfo")]
    public async Task<ActionResult<LoadpointInfo>> GetLoadpointInfoAsync() => await source.GetActiveLoadpointInfoAsync(interfaceLogger);
}