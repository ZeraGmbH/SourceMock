using Microsoft.Extensions.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public class SimulatedSource : ACSourceMock, ISimulatedSource
{
    public SimulatedSource(ILogger<SimulatedSource> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator) : base(logger, sourceCapabilities, validator)
    {
    }

    public SimulatedSource(ILogger<SimulatedSource> logger, ISourceCapabilityValidator validator) : base(logger, validator)
    {
    }

    private SimulatedSourceState? _simulatedSourceState;

    /// <inheritdoc/>
    public void SetSimulatedSourceState(SimulatedSourceState simulatedSourceState) => _simulatedSourceState = simulatedSourceState;

    /// <inheritdoc/>
    public SimulatedSourceState? GetSimulatedSourceState() => _simulatedSourceState;
}