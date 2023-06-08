using Microsoft.AspNetCore.Mvc;

using SourceMock.Actions.LoadpointValidator;
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
        /// <summary>
        /// Constructor for a SouceController.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        public SourceController(ILogger<SourceController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Sets a loadpoint without turning on the source.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the loadpoint could be set correctly.</response>
        /// <response code="400">If the loadpoint was malformed.</response>
        /// <response code="422">If the loadpoint was wellformed but invalid.</response>
        [HttpPost("Set")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult SetValues([FromBody] Loadpoint loadpoint)
        {
            var validationResult = LoadpointValidator.Validate(loadpoint);
            if (validationResult != LoadpointValidator.ValidationResult.OK)
            {
                logger.LogDebug($"Loadpoint validation failed with: {validationResult.ToString()}");
                return Problem(detail: validationResult.ToString(), statusCode: StatusCodes.Status422UnprocessableEntity);
            }

            return Ok();
        }

        /// <summary>
        /// Turns on the source with the previously set loadpoint.
        /// </summary>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the loadpoint was set successfully.</response>
        /// <response code="409">If the loadpoint could not be set successfully.</response>
        /// <response code="424">If no loadpoint is currently set.</response>
        [HttpPost("TurnOn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status424FailedDependency)]
        public IActionResult TurnOn()
        {
            return Ok();
        }

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The result of the operation, see responses.</returns>
        /// <response code="200">If the source was turned off successfully.</response>
        /// <response code="409">If the source could not be turned off successfully.</response>
        [HttpPost("TurnOff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult TurnOff()
        {
            return Ok();
        }
    }
}
