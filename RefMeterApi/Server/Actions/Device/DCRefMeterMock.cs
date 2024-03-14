using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class DCRefMeterMock : RefMeterMock
{
    private readonly IServiceProvider _di;

    /// <summary>
    /// 
    /// </summary>
    public DCRefMeterMock(IServiceProvider di)
    {
        _di = di;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lp"></param>
    /// <returns></returns>
    public override MeasuredLoadpoint CalcMeasureOutput(TargetLoadpoint lp)
    {
        var measureOutputPhases = new List<MeasuredLoadpointPhase>();

        var current = lp.Phases[0].Current.On ? lp.Phases[0].Current.DcComponent : 0;
        var voltage = lp.Phases[0].Voltage.On ? lp.Phases[0].Voltage.DcComponent : 0;

        var apparentPower = current * voltage;

        var measureOutputPhase = new MeasuredLoadpointPhase()
        {
            Current = new()
            {
                DcComponent = current
            },
            Voltage = new()
            {
                DcComponent = voltage
            },
            ApparentPower = apparentPower
        };

        measureOutputPhases.Add(measureOutputPhase);

        return new()
        {
            Phases = measureOutputPhases,
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstActiveVoltagePhase">Not relevant in dc</param>
    /// <returns></returns>
    public async override Task<MeasuredLoadpoint> GetActualValues(int firstActiveVoltagePhase = -1)
    {
        var loadpoint = await GetLoadpoint();

        var mo = CalcMeasureOutput(loadpoint);

        CalculateDeviations(mo);

        return mo;
    }

    private async Task<TargetLoadpoint> GetLoadpoint()
    {
        var r = Random.Shared;

        var source = _di.GetRequiredService<ISource>();

        var loadpoint = source.GetCurrentLoadpoint() ?? new TargetLoadpoint()
        {
            Phases = [
                new () {
                    Current = new() { DcComponent = 0},
                    Voltage = new() { DcComponent = 0},
                },
            ]
        };

        loadpoint = SharedLibrary.Utils.DeepCopy(loadpoint);

        var info = source.GetActiveLoadpointInfo();
        var currentSwitchedOffForDosage = await source.CurrentSwitchedOffForDosage();

        var phase = loadpoint.Phases[0];
        if (phase.Voltage.On && info.IsActive == false) phase.Voltage.On = false;
        if (phase.Current.On && (info.IsActive == false || currentSwitchedOffForDosage)) phase.Current.On = false;


        return loadpoint;
    }

    private static void CalculateDeviations(MeasuredLoadpoint mo)
    {
        mo.Phases[0].Voltage.DcComponent = GetRandomNumberWithPercentageDeviation(mo.Phases[0].Voltage.DcComponent ?? 0, 0.01);
        mo.Phases[0].Current.DcComponent = GetRandomNumberWithPercentageDeviation(mo.Phases[0].Current.DcComponent ?? 0, 0.01);
        mo.Phases[0].ApparentPower = GetRandomNumberWithPercentageDeviation(mo.Phases[0].ApparentPower!.Value, 0.01);
    }
}
