using SourceMock.Model;

namespace SourceMock.Actions.Source
{
    /// <summary>
    /// Simulatetes the behaviour of a ZERA source.
    /// </summary>
    public class SimulatedSource : ISource
    {
        private ILogger<SimulatedSource> logger;
        private IConfiguration configuration;

        /// <summary>
        /// Constructor that injects logger and configuration.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        /// <param name="configuration">The configuration o be used.</param>
        public SimulatedSource(ILogger<SimulatedSource> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public SourceResult SetLoadpoint(Loadpoint loadpoint)
        {
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOff()
        {
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOn()
        {
            return SourceResult.SUCCESS;
        }
    }
}