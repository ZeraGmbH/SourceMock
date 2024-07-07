using Microsoft.Extensions.Logging;
using SharedLibrary.DomainSpecific;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.SimulatedSource;

/// <summary>
/// 
/// </summary>
public interface IDCSourceMock : ISource { }

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="logger"></param>
/// <param name="validator"></param>
public class DCSourceMock(ILogger<DCSourceMock> logger, ISourceCapabilityValidator validator) :
    SourceMock(logger, new() { Phases = [new() { DcVoltage = new(new(10), new(300), new(0.01)), DcCurrent = new(new(0), new(60), new(0.01)) }] }, validator), IDCSourceMock
{
    public override Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        var power = (_loadpoint!.Phases[0].Voltage.DcComponent ?? Voltage.Zero) * (_loadpoint!.Phases[0].Current.DcComponent ?? Current.Zero);
        var elapsed = new Time((DateTime.Now - _startTime).TotalSeconds);
        var energy = power.GetActivePower(Angle.Zero) * elapsed;

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
