using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Models;
using SharedLibrary;
using SharedLibrary.DomainSpecific;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface IDCRefMeterMock : IMockRefMeter
{

}

/// <summary>
/// 
/// </summary>
public class DCRefMeterMock(IServiceProvider di) : RefMeterMock, IDCRefMeterMock
{
    private readonly IServiceProvider _di = di;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lp"></param>
    /// <returns></returns>
    public override MeasuredLoadpoint CalcMeasureOutput(TargetLoadpoint lp)
    {
        var measureOutputPhases = new List<MeasuredLoadpointPhase>();

        var current = lp.Phases[0].Current.On ? lp.Phases[0].Current.DcComponent : Current.Zero;
        var voltage = lp.Phases[0].Voltage.On ? lp.Phases[0].Voltage.DcComponent : Voltage.Zero;

        var activePower = (current != null && voltage != null)
            ? (current.Value * voltage.Value).GetActivePower(Angle.Zero)
            : ActivePower.Zero;

        var measureOutputPhase = new MeasuredLoadpointPhase()
        {
            Current = new() { DcComponent = current },
            Voltage = new() { DcComponent = voltage },
            ActivePower = activePower
        };

        measureOutputPhases.Add(measureOutputPhase);

        return new()
        {
            Phases = measureOutputPhases,
            ActivePower = activePower
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="firstActiveVoltagePhase">Not relevant in dc</param>
    /// <returns></returns>
    public async override Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveVoltagePhase = -1)
    {
        var loadpoint = await GetLoadpoint(logger);

        var mo = CalcMeasureOutput(loadpoint);

        CalculateDeviations(mo);

        return mo;
    }

    private async Task<TargetLoadpoint> GetLoadpoint(IInterfaceLogger logger)
    {
        var r = Random.Shared;

        var source = _di.GetRequiredService<ISource>();

        var loadpoint = source.GetCurrentLoadpoint(logger) ?? new TargetLoadpoint()
        {
            Phases = [
                new () {
                    Current = new() { DcComponent =Current.Zero },
                    Voltage = new() { DcComponent = Voltage.Zero },
                },
            ]
        };

        loadpoint = LibUtils.DeepCopy(loadpoint);

        var info = source.GetActiveLoadpointInfo(logger);
        var currentSwitchedOffForDosage = await source.CurrentSwitchedOffForDosage(logger);

        var phase = loadpoint.Phases[0];
        if (phase.Voltage.On && info.IsActive == false) phase.Voltage.On = false;
        if (phase.Current.On && (info.IsActive == false || currentSwitchedOffForDosage)) phase.Current.On = false;

        return loadpoint;
    }

    private static void CalculateDeviations(MeasuredLoadpoint mo)
    {
        var voltage = mo.Phases[0].Voltage.DcComponent;
        var current = mo.Phases[0].Current.DcComponent;
        var activePower = (current != null && voltage != null) ? (current.Value * voltage.Value).GetActivePower(new Angle()) : new();


        if (voltage != null)
            mo.Phases[0].Voltage.DcComponent = GetRandomNumberWithDeviation(voltage.Value, (Voltage)new(0.01)).Abs();
        if (current != null)
            mo.Phases[0].Current.DcComponent = GetRandomNumberWithDeviation(current.Value, (Current)new(0.01)).Abs();

        if ((double)activePower != 0)
        {
            mo.Phases[0].ActivePower = GetRandomNumberWithDeviation(activePower, (ActivePower)new(0.01));
            mo.ActivePower = mo.Phases[0].ActivePower;
        }
    }
}
