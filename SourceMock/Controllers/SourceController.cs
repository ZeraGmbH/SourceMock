using Microsoft.AspNetCore.Mvc;

using SourceMock.Actions.LoadpointValidator;
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
        /// Sets a loadpoint without turning on the source.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the loadpoint could be set correctly.</response>
        /// <response code="400">If the loadpoint was malformed.</response>
        /// <response code="422">If the loadpoint was wellformed but invalid.</response>
        /// <response code="500">If an unexpected error occured.</response>
        [HttpPost("SetLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult SetLoadpoint([FromBody] Loadpoint loadpoint)
        {
            var validationResult = LoadpointValidator.Validate(loadpoint);
            if (validationResult != LoadpointValidator.ValidationResult.OK)
            {
                _logger.LogDebug("Loadpoint validation failed with: {result}.", validationResult.ToString());
                return Problem(
                    detail: validationResult.ToString(),
                    statusCode: StatusCodes.Status422UnprocessableEntity);
            }

#pragma warning disable IDE0066 // Not all enum values are appicable here
            switch (_source.SetLoadpoint(loadpoint))
            {
                case SourceResult.SUCCESS:
                    return Ok();
                case SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES:
                    return Problem(
                        detail: SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES.ToString(),
                        statusCode: StatusCodes.Status422UnprocessableEntity);
                default:
                    return Problem(
                        detail: "Unkown Response from source.",
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
#pragma warning disable IDE0066 // Not all enum values are appicable here
            switch (_source.TurnOn())
            {
                case SourceResult.SUCCESS:
                    return Ok();
                case SourceResult.NO_LOADPOINT_SET:
                    return Problem(
                        detail: SourceResult.NO_LOADPOINT_SET.ToString(),
                        statusCode: StatusCodes.Status424FailedDependency);
                default:
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
#pragma warning disable IDE0066 // Not all enum values are appicable here
            switch (_source.TurnOff())
            {
                case SourceResult.SUCCESS:
                    return Ok();
                default:
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
        /// <response code="404">If there is no next loadpoint set, yet.</response>
        [HttpGet("NextLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Loadpoint> GetNextLoadpoint()
        {
            var loadpoint = _source.GetNextLoadpoint();

            return loadpoint == null
                ? (ActionResult<Loadpoint>)Problem(detail: "No next loadpoint was set, yet.", statusCode: StatusCodes.Status404NotFound)
                : (ActionResult<Loadpoint>)Ok(loadpoint);
        }

        /// <summary>
        /// Gets the currently active loadpoint.
        /// </summary>
        /// <returns>The loadpoint. HTTP 404 when the source is turned off.</returns>
        /// <response code="200">If there is a loadpoint to be returned.</response>
        /// <response code="404">If the source is turned off.</response>
        [HttpGet("CurrentLoadpoint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Loadpoint> GetCurrentLoadpoint()
        {
            var loadpoint = _source.GetCurrentLoadpoint();

            return loadpoint == null
                ? (ActionResult<Loadpoint>)Problem(detail: "The source is currently turned off.", statusCode: StatusCodes.Status404NotFound)
                : (ActionResult<Loadpoint>)Ok(loadpoint);
        }
    }
}
