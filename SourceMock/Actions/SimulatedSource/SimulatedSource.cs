namespace SourceMock.Actions.SimulatedSource
{
    /// <summary>
    /// Simulatetes the behaviour of a ZERA source.
    /// </summary>
    public class SimulatedSource : ISimulatedSource
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
    }
}