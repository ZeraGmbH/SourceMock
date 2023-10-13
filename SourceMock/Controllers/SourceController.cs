using Microsoft.AspNetCore.Mvc;

using SourceMock.Actions.Source;
using SourceMock.Model;

namespace SourceMock.Controllers
{
    /// <summary>
    /// Controls a source.
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SourceController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISource _source;

        /// <summary>
        /// Constructor for a SouceController.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="source">Injected source.</param>
        public SourceController(ILogger<SourceController> logger, ISource source)
        {
            _logger = logger;
            _source = source;
        }

        /// <summary>
        /// Gets the capabilities of this source.
        /// </summary>
        /// <returns>The corresponding<see cref="SourceCapabilities"/>-Object for this source.</returns>
        /// /// <response code="200">If the capabilities could be returned successfully.</response>
        [HttpGet("Capabilities")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<SourceCapabilities> GetCapablities()
        {
            return Ok(_source.GetCapabilities());
        }

        /// <summary>
        /// Sets a loadpoint without turning on the source.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the loadpoint could be set correctly.</response>
        /// <response code="400">If the loadpoint was (generally) malformed. The loadpoint cannot be set with any source.</response>
        /// <response code="422">If the loadpoint was wellformed but invalid. The loadpoint cannot be set with this source.</response>
        /// <response code="500">If an unexpected error occured.</response>
        [HttpPost("SetLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SetLoadpoint([FromBody] Loadpoint loadpoint)
        {
            _logger.LogTrace("Loadpoint to be set: ", loadpoint);

            var srcResult = _source.SetLoadpoint(loadpoint);

            switch (srcResult)
            {
                case SourceResult.SUCCESS:
                    _logger.LogTrace("Loadpoint was successfully set.");
                    return Ok();
                case SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES:
                case SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID:
                case SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID:
                case SourceResult.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS:
                    _logger.LogInformation(srcResult.ToString());
                    return Problem(
                        detail: srcResult.ToUserFriendlyString(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                default:
                    _logger.LogError($"Unkown response from source: ", srcResult.ToString());
                    return Problem(
                        detail: $"Unkown Response from source: {srcResult}, {srcResult.ToUserFriendlyString()}",
                        statusCode: StatusCodes.Status500InternalServerError);
            }
#pragma warning restore
        }

        /// <summary>
        /// Turns on the source with the previously set loadpoint.
        /// </summary>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the loadpoint was set successfully.</response>
        /// <response code="409">If the loadpoint could not be set successfully.</response>
        /// <response code="424">If no loadpoint is currently set.</response>
        /// <response code="500">If an unexpected error occured.</response>
        [HttpPost("TurnOn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status424FailedDependency)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult TurnOn()
        {
            var srcResult = _source.TurnOn();
#pragma warning disable IDE0066 // Not all enum values are appicable here
            switch (srcResult)
            {
                case SourceResult.SUCCESS:
                    _logger.LogTrace("The source was turned on.");
                    return Ok();
                case SourceResult.NO_LOADPOINT_SET:
                    _logger.LogInformation("The source could not be turned on because no loadpoint was set prevoiously.");
                    return Problem(
                        detail: SourceResult.NO_LOADPOINT_SET.ToString(),
                        statusCode: StatusCodes.Status424FailedDependency);
                default:
                    _logger.LogError($"Unkown response from source: ", srcResult.ToString());
                    return Problem(
                        detail: "Unkown Response from source.",
                        statusCode: StatusCodes.Status500InternalServerError);
            }
#pragma warning restore
        }

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the source was turned off successfully.</response>
        /// <response code="409">If the source could not be turned off successfully.</response>
        /// <response code="500">If an unexpected error occured.</response>
        [HttpPost("TurnOff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult TurnOff()
        {
            var srcResult = _source.TurnOff();
#pragma warning disable IDE0066 // Not all enum values are appicable here
            switch (srcResult)
            {
                case SourceResult.SUCCESS:
                    _logger.LogTrace("Source was turned off.");
                    return Ok();
                default:
                    _logger.LogError($"Unkown response from source: ", srcResult.ToString());
                    return Problem(
                        detail: "Unkown Response from source.",
                        statusCode: StatusCodes.Status500InternalServerError);
            }
#pragma warning restore
        }

        /// <summary>
        /// Gets the next loadpoint that would be set if "TurnOn" would be called.
        /// </summary>
        /// <returns>The loadpoint. HTTP 404 when no loadpoint is set.</returns>
        /// <response code="200">If there is a loadpoint to be returned.</response>
        /// <response code="204">If there is no next loadpoint set, yet.</response>
        [HttpGet("NextLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<Loadpoint> GetNextLoadpoint()
        {
            var loadpoint = _source.GetNextLoadpoint();

            return loadpoint == null
                ? NoContent()
                : Ok(loadpoint);
        }

        /// <summary>
        /// Gets the currently active loadpoint.
        /// </summary>
        /// <returns>The loadpoint. HTTP 204 when the source is turned off.</returns>
        /// <response code="200">If there is a loadpoint to be returned.</response>
        /// <response code="204">If the source is turned off.</response>
        [HttpGet("CurrentLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<Loadpoint> GetCurrentLoadpoint()
        {
            var loadpoint = _source.GetCurrentLoadpoint();

            return loadpoint == null
                ? NoContent()
                : Ok(loadpoint);
        }
    }
}
