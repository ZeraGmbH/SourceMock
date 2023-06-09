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
    public class SourceSimulationController : Controller
    {
        #region ConstructorAndDependencyInjection
        private readonly ILogger _logger;
        private readonly ISimulatedSource _source;

        /// <summary>
        /// Constructor for a SourceSimulationController.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        /// <param name="source">Injected simulated source.</param>
        public SourceSimulationController(ILogger<SourceController> logger, ISimulatedSource source)
        {
            _logger = logger;
            _source = source;
        }
        #endregion

        /// <summary>
        /// Sets the state of the simulated source.
        /// </summary>
        /// <returns></returns>
        [HttpPost("SourceState")]
        public ActionResult SetSourceState()
        {
            return Ok();
        }
    }
}