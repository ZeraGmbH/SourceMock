using SourceMock.Model;

namespace SourceMock.Actions.Source
{
    /// <summary>
    /// Simulatetes the behaviour of a ZERA source.
    /// </summary>
    public class SimulatedSource : ISource, ISimulatedSource
    {
        #region ContructorAndDependencyInjection
        private readonly ILogger<SimulatedSource> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor that injects logger and configuration.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        /// <param name="configuration">The configuration o be used.</param>
        public SimulatedSource(ILogger<SimulatedSource> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }
        #endregion

        private Loadpoint? _currentLoadpoint, _nextLoadpoint;
        private const string CONFIG_KEY_NUMBER_OF_PHASES = "SourceProperties:NumberOfPhases";
        private SimulatedSourceState? _simulatedSourceState;

        /// <inheritdoc/>
        public SourceResult SetLoadpoint(Loadpoint loadpoint)
        {
            if (loadpoint.Currents.Count() != _configuration.GetValue<int>(CONFIG_KEY_NUMBER_OF_PHASES))
            {
                return SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES;
            }

            _logger.LogTrace("Loadpoint set.");
            _nextLoadpoint = loadpoint;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOn()
        {
            if (_nextLoadpoint == null)
            {
                return SourceResult.NO_LOADPOINT_SET;
            }

            _logger.LogTrace("Source turned on.");
            _currentLoadpoint = _nextLoadpoint;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public SourceResult TurnOff()
        {
            _logger.LogTrace("Source turned off.");
            _currentLoadpoint = null;
            return SourceResult.SUCCESS;
        }

        /// <inheritdoc/>
        public Loadpoint? GetNextLoadpoint()
        {
            return _nextLoadpoint;
        }

        /// <inheritdoc/>
        public Loadpoint? GetCurrentLoadpoint()
        {
            return _currentLoadpoint;
        }

        /// <inheritdoc/>
        public void SetSimulatedSourceState(SimulatedSourceState simulatedSourceState)
        {
            _simulatedSourceState = simulatedSourceState;
        }

        /// <inheritdoc/>
        public SimulatedSourceState? GetSimulatedSourceState()
        {
            return _simulatedSourceState;
        }
    }
}