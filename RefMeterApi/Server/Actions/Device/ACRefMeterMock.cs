using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Models;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
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
public partial class ACRefMeterMock(IServiceProvider di) : RefMeterMock
{
    private readonly IServiceProvider _di = di;
    private const double PI_BY_180 = Math.PI / 180;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public override async Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveVoltagePhase = -1)
    {
        var loadpoint = await GetLoadpointAsync(logger);

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
        var activePowerSum = ActivePower.Zero;
        var reactivePowerSum = ReactivePower.Zero;
        var apparentPowerSum = ApparentPower.Zero;

        var measureOutputPhases = new List<MeasuredLoadpointPhase>();

        foreach (var phase in lp.Phases)
        {
            var current = phase.Current.On ? phase.Current.AcComponent!.Rms : Current.Zero;
            var voltage = phase.Voltage.On ? phase.Voltage.AcComponent!.Rms : Voltage.Zero;
            var power = current * voltage;

            var angle = (phase.Current.AcComponent!.Angle - phase.Voltage.AcComponent!.Angle).Abs();

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
                ActivePower = power.GetActivePower(angle),
                ReactivePower = power.GetReactivePower(angle),
                ApparentPower = power
            };

            measureOutputPhase.PowerFactor = (double)measureOutputPhase.ApparentPower == 0
                ? new PowerFactor(1)
                : measureOutputPhase.ActivePower.Value / measureOutputPhase.ApparentPower.Value;

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


    private async Task<TargetLoadpoint> GetLoadpointAsync(IInterfaceLogger logger)
    {
        var r = Random.Shared;

        var source = _di.GetRequiredService<ISource>();
        var hasSource = await source.GetAvailableAsync(logger);

        var sourceLoadPoint = hasSource ? await source.GetCurrentLoadpointAsync(logger) : new TargetLoadpoint()
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

        var loadpoint = sourceLoadPoint ?? new TargetLoadpoint()
        {
            Frequency = new() { Value = new(0) },
            Phases = [
                new () {
                    Current = new() { AcComponent = new() { Rms = new(0), Angle=new(0)}},
                    Voltage = new() { AcComponent = new() { Rms = new(0), Angle=new(0)}},
                },
                new () {
                    Current = new() { AcComponent = new() { Rms = new(0), Angle=new(249)}},
                    Voltage = new() { AcComponent = new() { Rms = new(0), Angle=new(240)}},
                },
                new () {
                    Current = new() { AcComponent = new() { Rms = new(0), Angle=new(120)}},
                    Voltage = new() { AcComponent = new() { Rms = new(0), Angle=new(120)}},
                }
            ]
        };

        loadpoint = LibUtils.DeepCopy(loadpoint);

        if (!hasSource) return loadpoint;

        var info = await source.GetActiveLoadpointInfoAsync(logger);
        var currentSwitchedOffForDosage = await source.CurrentSwitchedOffForDosageAsync(logger);

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
        mo.Frequency = GetRandomNumberWithDeviation(mo.Frequency!.Value, 0.02);
        mo.ActivePower = GetRandomNumberWithDeviation(mo.ActivePower!.Value, (ActivePower)new(0.02));
        mo.ApparentPower = GetRandomNumberWithDeviation(mo.ApparentPower!.Value, (ApparentPower)new(0.02));
        mo.ReactivePower = GetRandomNumberWithDeviation(mo.ReactivePower!.Value, (ReactivePower)new(0.02));
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

        phase.Current.AcComponent!.Rms = !phase.Current.AcComponent!.Rms
            ? GetRandomNumberWithDeviation(phase.Current.AcComponent!.Rms, (Current)new(0.01)).Abs()
            : GetRandomNumberWithDeviation(phase.Current.AcComponent!.Rms, 0.01);
        phase.Current.AcComponent!.Angle = GetRandomNumberWithDeviation(phase.Current.AcComponent!.Angle, (Angle)new(0.1)).Abs().Normalize();
        phase.Voltage.AcComponent!.Rms = !phase.Voltage.AcComponent!.Rms
            ? GetRandomNumberWithDeviation(phase.Voltage.AcComponent!.Rms, (Voltage)new(0.05)).Abs()
            : GetRandomNumberWithDeviation(phase.Voltage.AcComponent!.Rms, 0.05);
        phase.Voltage.AcComponent!.Angle = !phase.Voltage.AcComponent!.Angle
            ? GetRandomNumberWithDeviation(phase.Voltage.AcComponent!.Angle, (Angle)new(0.05)).Abs().Normalize()
            : GetRandomNumberWithDeviation(phase.Voltage.AcComponent!.Angle, 0.05);
        phase.ActivePower = GetRandomNumberWithDeviation(phase.ActivePower!.Value, (ActivePower)new(0.02));
        phase.ReactivePower = GetRandomNumberWithDeviation(phase.ReactivePower!.Value, (ReactivePower)new(0.02));
        phase.ApparentPower = GetRandomNumberWithDeviation(phase.ApparentPower!.Value, (ApparentPower)new(0.02));
        phase.PowerFactor = PowerFactorIsNotNullOrNan(phase.PowerFactor)
            ? GetRandomNumberWithDeviation(phase.PowerFactor!.Value, new(0.02), new(-1), new(+1))
            : GetRandomNumberWithDeviation(new PowerFactor(0), (PowerFactor)new(0.01));
    }

    private static bool PowerFactorIsNotNullOrNan(PowerFactor? powerFactor)
        => powerFactor != null && !!powerFactor.Value && !powerFactor.Value.IsNaN();

    /// <inheritdoc/>
    public override Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger)
    {
        return Task.FromResult(new ReferenceMeterInformation
        {
            Model = "ACRefMeterMock",
            NumberOfPhases = 3,
            SerialNumber = "29091963",
            SoftwareVersion = "0.1",
            SupportsManualRanges = true,
            SupportsPllChannelSelection = true
        });
    }
}
