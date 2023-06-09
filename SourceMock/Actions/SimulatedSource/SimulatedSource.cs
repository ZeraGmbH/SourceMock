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


        private Loadpoint? currentLoadpoint, nextLoadpoint;
        private const string CONFIG_KEY_NUMBER_OF_PHASES = "SourceProperties:NumberOfPhases";

        /// <inheritdoc/>
        public SourceResult SetLoadpoint(Loadpoint loadpoint)
        {
            if (loadpoint.Currents.Count() != configuration.GetValue<int>(CONFIG_KEY_NUMBER_OF_PHASES))
            {
                return SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES;
            }

            nextLoadpoint = loadpoint;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOn()
        {
            if (nextLoadpoint == null)
            {
                return SourceResult.NO_LOADPOINT_SET;
            }

            currentLoadpoint = nextLoadpoint;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOff()
        {
            currentLoadpoint = null;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public Loadpoint? GetNextLoadpoint()
        {
            return nextLoadpoint;
        }

        /// <inheritdoc/>
        public Loadpoint? GetCurrentLoadpoint()
        {
            return currentLoadpoint;
        }
    }
}