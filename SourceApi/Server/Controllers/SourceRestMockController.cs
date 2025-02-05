
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SourceApi.Actions.Source;
using ZERA.WebSam.Shared.Models.Source;
using Swashbuckle.AspNetCore.Annotations;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Provider;

namespace SourceApi.Controllers;

/// <summary>
/// Controls a source.
/// </summary>
/// <remarks>
/// Constructor for a SouceController.
/// </remarks>
/// <param name="logger">Injected logger.</param>
/// <param name="source">Injected source.</param>
/// <param name="interfaceLogger"></param>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/srcrestmock")]
public class SourceRestMockController([FromKeyedServices(SourceRestMockController.MockKey)] ISource source, ILogger<SourceController> logger, IInterfaceLogger interfaceLogger) : Controller
{
    /// <summary>
    /// 
    /// </summary>
    public const string MockKey = "RESTMOCK";

    /// <summary>
    /// Gets the capabilities of this source.
    /// </summary>
    /// <returns>The corresponding<see cref="SourceCapabilities"/>-Object for this source.</returns>
    [HttpGet("Capabilities"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetCapabilities")]
    public async Task<ActionResult<SourceCapabilities>> GetCapablitiesAsync() => Ok(await source.GetCapabilitiesAsync(interfaceLogger));

    /// <summary>
    /// Gets the capabilities of this source.
    /// </summary>
    /// <returns>The corresponding<see cref="SourceCapabilities"/>-Object for this source.</returns>
    [HttpGet("Available"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SourceIsAvailable")]
    public async Task<ActionResult<bool>> IsAvailableAsync() => Ok(await source.GetAvailableAsync(interfaceLogger));

    /// <summary>
    /// Sets a loadpoint without turning on the source.
    /// </summary>
    /// <param name="loadpoint">The loadpoint to be set.</param>
    /// <returns>The result of the operation, see responses.</returns>
    [HttpPut("Loadpoint"), AllowAnonymous]
    [SwaggerOperation(OperationId = "SetLoadpoint")]
    public async Task<ActionResult> SetLoadpointAsync([FromBody] TargetLoadpoint loadpoint)
    {
        logger.LogTrace($"Loadpoint to be set: {loadpoint}");

        var srcResult = await source.SetLoadpointAsync(interfaceLogger, loadpoint);

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
    [HttpPost("TurnOff"), AllowAnonymous]
    [SwaggerOperation(OperationId = "TurnOff")]
    public async Task<ActionResult> TurnOffAsync()
    {
        var srcResult = await source.TurnOffAsync(interfaceLogger);

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
    [HttpGet("Loadpoint"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetLoadpoint")]
    public ActionResult<TargetLoadpoint> GetCurrentLoadpoint()
    {
        var loadpoint = source.GetCurrentLoadpointAsync(interfaceLogger);

        return loadpoint == null
            ? NoContent()
            : Ok(loadpoint);
    }

    /// <summary>
    /// Report information on the last loadpoint activated.
    /// </summary>
    /// <returns>The information including some dates.</returns>
    [HttpGet("LoadpointInfo"), AllowAnonymous]
    [SwaggerOperation(OperationId = "GetLoadpointInfo")]
    public async Task<ActionResult<LoadpointInfo>> GetLoadpointInfoAsync() => await source.GetActiveLoadpointInfoAsync(interfaceLogger);
}