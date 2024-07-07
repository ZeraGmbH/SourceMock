using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Controllers
{
    /// <summary>
    /// Controls a source.
    /// </summary>
    /// <param name="logger">Injected logger.</param>
    /// <param name="source">Injected simulated source.</param>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SourceSimulationController(ILogger<SourceController> logger, ISimulatedSource source) : Controller
    {
        #region ConstructorAndDependencyInjection
        private readonly ILogger _logger = logger;
        private readonly ISimulatedSource _source = source;
        #endregion

        /// <summary>
        /// Returns the current state of the source.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">If the source state retrieval was successful.</response>
        /// <response code="404">If no source state was set yet.</response>
        [HttpGet("SourceState")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "GetSourceState")]
        public ActionResult<SimulatedSourceState> GetSourceState()
        {
            var state = _source.GetSimulatedSourceState();
            return state == null
                ? Problem(detail: "No state was set yet.", statusCode: StatusCodes.Status404NotFound)
                : Ok(state);
        }

        /// <summary>
        /// Sets the state of the simulated source.
        /// </summary>
        /// <param name="simulatedSourceState">The state to be set.</param>
        /// <returns></returns>
        /// <response code="200">If the source state was successfully set.</response>
        [HttpPost("SourceState")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(OperationId = "SetSourceState")]
        public ActionResult SetSourceState([FromBody] SimulatedSourceState simulatedSourceState)
        {
            _source.SetSimulatedSourceState(simulatedSourceState);
            _logger.LogTrace($"New simulated source state set: {simulatedSourceState}");
            return Ok();
        }
    }
}