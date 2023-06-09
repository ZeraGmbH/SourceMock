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
        private ILogger logger;
        private ISource source;

        /// <summary>
        /// Constructor for a SouceController.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="source">Injected source.</param>
        public SourceController(ILogger<SourceController> logger, ISource source)
        {
            this.logger = logger;
            this.source = source;
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
                logger.LogDebug($"Loadpoint validation failed with: {validationResult.ToString()}");
                return Problem(detail: validationResult.ToString(), statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            switch (source.SetLoadpoint(loadpoint))
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
            switch (source.TurnOn())
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
            switch (source.TurnOff())
            {
                case SourceResult.SUCCESS:
                    return Ok();
                default:
                    return Problem(
                        detail: "Unkown Response from source.",
                        statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
