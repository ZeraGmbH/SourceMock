using Microsoft.Extensions.Logging;
using SharedLibrary.DomainSpecific;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.SimulatedSource;

/// <summary>
/// 
/// </summary>
public interface IDCSourceMock : ISource
{

}

/// <summary>
/// 
/// </summary>
public class DCSourceMock : SourceMock, IDCSourceMock
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="validator"></param>
    public DCSourceMock(ILogger<DCSourceMock> logger, ISourceCapabilityValidator validator) : base(logger, new()
    {
        Phases = new() {
                    new() {
                        DcVoltage = new(10, 300, 0.01),
                        DcCurrent = new(0, 60, 0.01)
                    }
                }
    }, validator)
    { }

    public override Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, double meterConstant)
    {
        var power = (_loadpoint!.Phases[0].Voltage.DcComponent ?? 0) * (_loadpoint!.Phases[0].Current.DcComponent ?? 0);
        var elapsedHours = (DateTime.Now - _startTime).TotalHours;
        var energy = power * elapsedHours;

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

    public override Task<DosageProgress> GetDosageProgressNGX(IInterfaceLogger logger, MeterConstant meterConstant)
    {
        var power = (_loadpointNGX!.Phases[0].Voltage.DcComponent ?? 0) * (_loadpointNGX!.Phases[0].Current.DcComponent ?? 0);
        var elapsedHours = (DateTime.Now - _startTime).TotalHours;
        var energy = power * elapsedHours;

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
