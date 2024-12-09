using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source;

public interface IACSourceMock : ISource
{
}

/// <summary>
/// Simulatetes the behaviour of a ZERA source.
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="logger"></param>
/// <param name="sourceCapabilities"></param>
/// <param name="validator"></param>
public class ACSourceMock(ILogger<ACSourceMock> logger, SourceCapabilities sourceCapabilities, ISourceCapabilityValidator validator) : SourceMock(logger, sourceCapabilities, validator), IACSourceMock
{
    /// <summary>
    /// Constructor that injects logger and configuration and uses default source capablities.
    /// </summary>
    /// <param name="logger">The logger to be used.</param>
    /// <param name="validator">The validator to be used.</param>
    public ACSourceMock(ILogger<ACSourceMock> logger, ISourceCapabilityValidator validator) : this(logger, new()
    {
        Phases = [
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) },
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) },
            new() { AcVoltage = new(new(10), new(300), new(0.01)), AcCurrent = new(new(0), new(60), new(0.01)) }
        ],
        FrequencyRanges = [
                new(new(40), new(60), new(0.1), FrequencyMode.SYNTHETIC),
                new(new(15), new(20), new(0.1), FrequencyMode.SYNTHETIC),
                new(new(0), new(0), new(1), FrequencyMode.GRID_SYNCHRONOUS),
                new(new(0), new(0), new(0), FrequencyMode.THIRD_OF_GRID_SYNCHRONOUS),
            ]
    }, validator)
    { }

    public override Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        var power = ActivePower.Zero;

        foreach (var phase in _loadpoint!.Phases)
            if (phase.Voltage.On && phase.Current.On)
                power += (phase.Voltage.AcComponent!.Rms * phase.Current.AcComponent!.Rms).GetActivePower(phase.Voltage.AcComponent!.Angle - phase.Current.AcComponent!.Angle);

        var elapsed = new Time((DateTime.Now - _startTime).TotalSeconds);
        var energy = (power * elapsed).Abs();

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

    public override async Task NoSourceAsync(IInterfaceLogger logger)
    {
        await base.NoSourceAsync(logger);

        _loadpoint = new()
        {
            Frequency = new() { Value = new(50) },
            Phases = [
                new () {
                    Current = new() { On = true, AcComponent = new() { Rms = new(5), Angle=new(0)}},
                    Voltage = new() { On = true, AcComponent = new() { Rms = new(230), Angle=new(0)}},
                },
                new () {
                    Current = new() { On = true, AcComponent = new() { Rms = new(5), Angle=new(249)}},
                    Voltage = new() { On = true, AcComponent = new() { Rms = new(230), Angle=new(240)}},
                },
                new () {
                    Current = new() { On = true, AcComponent = new() { Rms = new(5), Angle=new(120)}},
                    Voltage = new() { On = true, AcComponent = new() { Rms = new(230), Angle=new(120)}},
                }
            ]
        };
    }

    public override Task<ActiveEnergy> GetEnergyAsync(IInterfaceLogger logger)
    {
        var power = ActivePower.Zero;

        foreach (var phase in _loadpoint!.Phases)
            if (phase.Voltage.On && phase.Current.On)
                power += (phase.Voltage.AcComponent!.Rms * phase.Current.AcComponent!.Rms).GetActivePower(phase.Voltage.AcComponent!.Angle - phase.Current.AcComponent!.Angle);

        var elapsed = new Time((DateTime.Now - _startTime).TotalSeconds);

        return Task.FromResult(power * elapsed);
    }
}