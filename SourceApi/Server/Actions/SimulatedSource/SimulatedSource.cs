using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public interface ISourceMock : ISource
{

}
/// <summary>
/// Simulatetes the behaviour of a ZERA source.
/// </summary>
public class SimulatedSource : SourceMock, ISimulatedSource
{

    private SimulatedSourceState? _simulatedSourceState;

    /// <summary>
    /// Constructor that injects logger and configuration and uses default source capablities.
    /// </summary>
    /// <param name="logger">The logger to be used.</param>
    /// <param name="validator">The validator to be used.</param>
    public SimulatedSource(ILogger<SimulatedSource> logger, ISourceCapabilityValidator validator) : base(logger, new()
    {
        Phases = [
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) },
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) },
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) }
        ],
        FrequencyRanges = [new(new(40), new(60), new(0.1), FrequencyMode.SYNTHETIC)]
    }, validator)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="sourceCapabilities"></param>
    /// <param name="validator"></param>
    public SimulatedSource(ILogger<SimulatedSource> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator) : base(logger, sourceCapabilities, validator)
    {
    }

    /// <inheritdoc/>
    public void SetSimulatedSourceState(SimulatedSourceState simulatedSourceState) => _simulatedSourceState = simulatedSourceState;

    /// <inheritdoc/>
    public SimulatedSourceState? GetSimulatedSourceState() => _simulatedSourceState;

    public override Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        var power = ActivePower.Zero;

        foreach (var phase in _loadpoint!.Phases)
            if (phase.Voltage.On && phase.Current.On)
                power += (phase.Voltage.AcComponent!.Rms * phase.Current.AcComponent!.Rms).GetActivePower(phase.Voltage.AcComponent!.Angle - phase.Current.AcComponent!.Angle);

        var elapsed = new Time((DateTime.Now - _startTime).TotalSeconds);
        var energy = power * elapsed;

        if (energy > _dosageEnergy) energy = DosageDone();

        _status.Progress = energy;
        _status.Remaining = _dosageEnergy - energy;
        _status.Total = _dosageEnergy;

        return Task.FromResult(new DosageProgress
        {
            Active = _status.Active,
            Progress = _status.Progress,
            Remaining = _status.Remaining,
            Total = _status.Total
        });
    }
}