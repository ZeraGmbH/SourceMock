using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Models;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// This is a tag interface which simply identifies specific 
/// variants.
/// </summary>
public interface IMockRefMeter : IRefMeter
{
}

/// <summary>
/// 
/// </summary>
public partial class ACRefMeterMock : RefMeterMock
{
    private readonly IServiceProvider _di;

    /// <summary>
    /// 
    /// </summary>
    public ACRefMeterMock(IServiceProvider di)
    {
        _di = di;
    }

    private const double PI_BY_180 = Math.PI / 180;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public override async Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveVoltagePhase = -1)
    {
        var loadpoint = await GetLoadpoint(logger);

        var mo = CalcMeasureOutput(loadpoint);

        foreach (var phase in mo.Phases)
            CalculatePhaseDeviations(phase);

        CalculateDeviations(mo);

        return mo;
    }

    /// <summary>
    /// Calculates an expected Measure Output from a given loadpoint.
    /// </summary>
    /// <param name="lp">The loadpoint.</param>
    /// <returns>The according measure output.</returns>
    public override MeasuredLoadpoint CalcMeasureOutput(TargetLoadpoint lp)
    {
        var activePowerSum = 0d;
        var reactivePowerSum = 0d;
        var apparentPowerSum = 0d;

        var measureOutputPhases = new List<MeasuredLoadpointPhase>();

        foreach (var phase in lp.Phases)
        {
            var current = phase.Current.On ? phase.Current.AcComponent!.Rms : 0;
            var voltage = phase.Voltage.On ? phase.Voltage.AcComponent!.Rms : 0;

            var angle = Math.Abs(phase.Current.AcComponent!.Angle - phase.Voltage.AcComponent!.Angle);

            var measureOutputPhase = new MeasuredLoadpointPhase()
            {
                Current = new()
                {
                    AcComponent = new()
                    {
                        Rms = current,
                        Angle = phase.Current.AcComponent!.Angle,
                    },
                },
                Voltage = new()
                {
                    AcComponent = new()
                    {
                        Rms = voltage,
                        Angle = phase.Voltage.AcComponent!.Angle,
                    },
                },
                ActivePower = current * voltage * Math.Cos(angle * PI_BY_180),
                ReactivePower = current * voltage * Math.Sin(angle * PI_BY_180),
                ApparentPower = current * voltage
            };

            measureOutputPhase.PowerFactor = measureOutputPhase.ApparentPower == 0
                ? 1
                : measureOutputPhase.ActivePower / measureOutputPhase.ApparentPower;

            activePowerSum += measureOutputPhase.ActivePower.Value;
            reactivePowerSum += measureOutputPhase.ReactivePower.Value;
            apparentPowerSum += measureOutputPhase.ApparentPower.Value;
            measureOutputPhases.Add(measureOutputPhase);
        }

        return new()
        {
            Frequency = lp.Frequency.Value,
            PhaseOrder = CalculatePhaseOrder(lp),
            Phases = measureOutputPhases,
            ActivePower = activePowerSum,
            ApparentPower = apparentPowerSum,
            ReactivePower = reactivePowerSum
        };
    }

    private async Task<TargetLoadpoint> GetLoadpoint(IInterfaceLogger logger)
    {
        var r = Random.Shared;

        var source = _di.GetRequiredService<ISource>();

        var loadpoint = source.GetCurrentLoadpoint() ?? new TargetLoadpoint()
        {
            Frequency = new Frequency() { Value = 0 },
            Phases = [
                new () {
                    Current = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                    Voltage = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                },
                new () {
                    Current = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                    Voltage = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                },
                new () {
                    Current = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                    Voltage = new() { AcComponent = new() { Rms = 0, Angle=r.Next(0,360)}},
                }
            ]
        };

        loadpoint = SharedLibrary.Utils.DeepCopy(loadpoint);

        var info = source.GetActiveLoadpointInfo();
        var currentSwitchedOffForDosage = await source.CurrentSwitchedOffForDosage(logger);

        foreach (var phase in loadpoint.Phases)
        {
            if (phase.Voltage.On && info.IsActive == false) phase.Voltage.On = false;
            if (phase.Current.On && (info.IsActive == false || currentSwitchedOffForDosage)) phase.Current.On = false;
        }

        return loadpoint;
    }

    /// <summary>
    /// Calculate the phase order - simply approach for results 123 and 132 only.
    /// </summary>
    /// <param name="lp">Some loadpoint.</param>
    /// <returns>The phase order.</returns>
    private static string CalculatePhaseOrder(TargetLoadpoint lp)
    {
        /* See if there are at least three phases - use very defensive programming, maybe a bit too much. */
        var phases = lp?.Phases?.Select(p => p.Current).Where(v => v != null).ToList();

        if (phases == null) return "123";

        var phaseCount = phases.Count;

        if (phaseCount < 3) return "123";

        /* Find the first angle - the first voltage seen by any consumer. */
        var minAngle = phases.Min(v => v.AcComponent!.Angle);
        var l1 = phases.FindIndex(v => v.AcComponent!.Angle == minAngle);

        /* Get the following phases and compare angles. */
        var l2 = (l1 + 1) % phaseCount;
        var l3 = (l2 + 1) % phaseCount;

        return phases[l2].AcComponent!.Angle < phases[l3].AcComponent!.Angle ? "132" : "123";
    }

    private static void MeasureOutputPhaseNullCheck(MeasuredLoadpointPhase phase)
    {
        _ = phase.Current ?? throw new ArgumentNullException();
        _ = phase.Current.AcComponent ?? throw new ArgumentNullException();
        _ = phase.Voltage ?? throw new ArgumentNullException();
        _ = phase.Voltage.AcComponent ?? throw new ArgumentNullException();
        _ = phase.ActivePower ?? throw new ArgumentNullException();
        _ = phase.ReactivePower ?? throw new ArgumentNullException();
        _ = phase.ApparentPower ?? throw new ArgumentNullException();
        _ = phase.PowerFactor ?? throw new ArgumentNullException();
    }

    private static void CalculateDeviations(MeasuredLoadpoint mo)
    {
        MeasureOutputNullCheck(mo);

        mo.Frequency = GetRandomNumberWithPercentageDeviation(mo.Frequency!.Value, 0.02);
        mo.ActivePower = GetRandomNumberWithAbsoluteDeviation(mo.ActivePower!.Value, 0.02);
        mo.ApparentPower = GetRandomNumberWithAbsoluteDeviation(mo.ApparentPower!.Value, 0.02);
        mo.ReactivePower = GetRandomNumberWithAbsoluteDeviation(mo.ReactivePower!.Value, 0.02);
    }

    private static void MeasureOutputNullCheck(MeasuredLoadpoint mo)
    {
        _ = mo.ActivePower ?? throw new ArgumentNullException();
        _ = mo.ApparentPower ?? throw new ArgumentNullException();
        _ = mo.ReactivePower ?? throw new ArgumentNullException();
        _ = mo.Frequency ?? throw new ArgumentException();
    }

    private static void CalculatePhaseDeviations(MeasuredLoadpointPhase phase)
    {
        MeasureOutputPhaseNullCheck(phase);

        phase.Current.AcComponent!.Rms = phase.Current.AcComponent!.Rms != 0
            ? GetRandomNumberWithPercentageDeviation(phase.Current.AcComponent!.Rms, 0.01)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Current.AcComponent!.Rms, 0.01));
        phase.Current.AcComponent!.Angle = Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Current.AcComponent!.Angle, 0.1));
        phase.Voltage.AcComponent!.Rms = phase.Voltage.AcComponent!.Rms != 0
            ? GetRandomNumberWithPercentageDeviation(phase.Voltage.AcComponent!.Rms, 0.05)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Voltage.AcComponent!.Rms, 0.05));
        phase.Voltage.AcComponent!.Angle = phase.Voltage.AcComponent!.Angle != 0
            ? GetRandomNumberWithPercentageDeviation(phase.Voltage.AcComponent!.Angle, 0.05)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Voltage.AcComponent!.Angle, 0.05));
        phase.ActivePower = GetRandomNumberWithAbsoluteDeviation(phase.ActivePower!.Value, 0.02);
        phase.ReactivePower = GetRandomNumberWithAbsoluteDeviation(phase.ReactivePower!.Value, 0.02);
        phase.ApparentPower = GetRandomNumberWithAbsoluteDeviation(phase.ApparentPower!.Value, 0.02);
        phase.PowerFactor = PowerFactorIsNotNullOrNan(phase.PowerFactor)
            ? GetRandomNumberWithAbsoluteDeviation(phase.PowerFactor!.Value, 0.02)
            : GetRandomNumberWithAbsoluteDeviation(0, 0.01);
    }

    private static bool PowerFactorIsNotNullOrNan(double? powerFactor)
    {
        if (powerFactor == null)
            return false;

        return powerFactor != 0 && !double.IsNaN((double)powerFactor);
    }
}
