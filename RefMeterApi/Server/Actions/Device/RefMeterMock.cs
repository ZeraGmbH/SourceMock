using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Models;
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
public partial class RefMeterMock : IMockRefMeter
{

    private MeasurementModes? _measurementMode = null;
    private readonly IServiceProvider _di;

    /// <summary>
    /// 
    /// </summary>
    public RefMeterMock(IServiceProvider di)
    {
        _di = di;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool Available => true;

    /// <summary>
    /// MeasurementMode
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        return Task.FromResult(_measurementMode);
    }

    private const double PI_BY_180 = Math.PI / 180;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>ActualValues that fluctuate around the set loadpoint</returns>
    public Task<MeasureOutput> GetActualValues(int firstActiveVoltagePhase = -1)
    {
        Loadpoint loadpoint = GetLoadpoint();
        MeasureOutput mo = CalcMeasureOutput(loadpoint);

        foreach (var phase in mo.Phases)
        {
            CalculatePhaseDeviations(phase);
        }

        CalculateDeviations(mo);

        return Task.FromResult(mo);
    }

    /// <summary>
    /// Calculates an expected Measure Output from a given loadpoint.
    /// </summary>
    /// <param name="lp">The loadpoint.</param>
    /// <returns>The according measure output.</returns>
    public static MeasureOutput CalcMeasureOutput(Loadpoint lp)
    {
        double activePowerSum = 0;
        double reactivePowerSum = 0;
        double apparentPowerSum = 0;

        var measureOutputPhases = new List<MeasureOutputPhase>();
        foreach (var phase in lp.Phases)
        {
            var current = phase.Current.Rms;
            var voltage = phase.Voltage.Rms;
            var angle = Math.Abs(phase.Current.Angle - phase.Voltage.Angle);

            var measureOutputPhase = new MeasureOutputPhase()
            {
                Current = phase.Current.Rms,
                AngleCurrent = phase.Current.Angle,
                Voltage = phase.Voltage.Rms,
                AngleVoltage = phase.Voltage.Angle,
                ActivePower = current * voltage * Math.Cos(angle * PI_BY_180),
                ReactivePower = current * voltage * Math.Sin(angle * PI_BY_180),
                ApparentPower = phase.Current.Rms * phase.Voltage.Rms
            };
            measureOutputPhase.PowerFactor = measureOutputPhase.ActivePower / measureOutputPhase.ApparentPower;

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

    /// <summary>
    /// Returns all entrys in enum MeasurementModes
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        return Task.FromResult((MeasurementModes[])Enum.GetValues(typeof(MeasurementModes)));
    }

    /// <summary>
    /// Measurement mode is not relevant for mock logic but frontent requeires an implementation
    /// </summary>
    /// <param name="mode">Real RefMeter requieres a mode</param>
    /// <returns>Must return something - no async task requeired without device</returns>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        _measurementMode = mode;
        return Task.FromResult<MeasurementModes>(mode);
    }

    private Loadpoint GetLoadpoint()
    {
        Random r = new();

        return _di.GetRequiredService<ISource>().GetCurrentLoadpoint() ?? new Loadpoint()
        {
            Frequency = new Frequency() { Value = 0 },
            Phases = new List<PhaseLoadpoint>(){
                new PhaseLoadpoint(){
                    Current = new() {Rms = 0, Angle=r.Next(0,360)},
                    Voltage = new() {Rms = 0, Angle=r.Next(0,360)},
                },
                new PhaseLoadpoint(){
                    Current = new() {Rms = 0, Angle=r.Next(0,360)},
                    Voltage = new() {Rms = 0, Angle=r.Next(0,360)},
                },
                new PhaseLoadpoint(){
                    Current = new() {Rms = 0, Angle=r.Next(0,360)},
                    Voltage = new() {Rms = 0, Angle=r.Next(0,360)},
                }
            }
        };
    }

    // Phases are sorted by the phase angles from lowest to highest
    private static string CalculatePhaseOrder(Loadpoint lp)
    {
        Dictionary<double, int> anglesWithIndices = new();
        List<double> orderedAngles = new();

        CopyAngleValues(lp, anglesWithIndices, orderedAngles);

        orderedAngles.Sort();

        string result = GetPhaseOrder(anglesWithIndices, orderedAngles);

        return result;
    }

    private static string GetPhaseOrder(Dictionary<double, int> anglesWithIndices, List<double> orderedAngles)
    {
        string result = "";
        for (int i = 0; i < anglesWithIndices.Count; ++i)
        {
            result += (anglesWithIndices[orderedAngles[i]] + 1).ToString();
        }

        return result;
    }

    private static void CopyAngleValues(Loadpoint lp, Dictionary<double, int> angles, List<double> orderedAngles)
    {
        for (int i = 0; i < lp.Phases.Count; ++i)
        {
            angles.Add(lp.Phases[i].Voltage.Angle, i);
            orderedAngles.Add(lp.Phases[i].Voltage.Angle);
        }
    }

    private static void MeasureOutputPhaseNullCheck(MeasureOutputPhase phase)
    {
        _ = phase.Current ?? throw new ArgumentNullException();
        _ = phase.AngleCurrent ?? throw new ArgumentNullException();
        _ = phase.Voltage ?? throw new ArgumentNullException();
        _ = phase.ActivePower ?? throw new ArgumentNullException();
        _ = phase.ReactivePower ?? throw new ArgumentNullException();
        _ = phase.ApparentPower ?? throw new ArgumentNullException();
        _ = phase.PowerFactor ?? throw new ArgumentNullException();
    }

    private static void CalculateDeviations(MeasureOutput mo)
    {
        MeasureOutputNullCheck(mo);

        mo.Frequency = GetRandomNumberWithPercentageDeviation(mo.Frequency!.Value, 0.02);
        mo.ActivePower = GetRandomNumberWithAbsoluteDeviation(mo.ActivePower!.Value, 0.02);
        mo.ApparentPower = GetRandomNumberWithAbsoluteDeviation(mo.ApparentPower!.Value, 0.02);
        mo.ReactivePower = GetRandomNumberWithAbsoluteDeviation(mo.ReactivePower!.Value, 0.02);
    }

    private static void MeasureOutputNullCheck(MeasureOutput mo)
    {
        _ = mo.ActivePower ?? throw new ArgumentNullException();
        _ = mo.ApparentPower ?? throw new ArgumentNullException();
        _ = mo.ReactivePower ?? throw new ArgumentNullException();
        _ = mo.Frequency ?? throw new ArgumentException();
    }

    private static void CalculatePhaseDeviations(MeasureOutputPhase phase)
    {
        MeasureOutputPhaseNullCheck(phase);

        phase.Current = phase.Current != 0
            ? GetRandomNumberWithPercentageDeviation(phase.Current!.Value, 0.01)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Current.Value, 0.01));
        phase.AngleCurrent = Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.AngleCurrent!.Value, 0.1));
        phase.Voltage = phase.Voltage != 0
            ? GetRandomNumberWithPercentageDeviation(phase.Voltage!.Value, 0.05)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.Voltage.Value, 0.05));
        phase.AngleVoltage = phase.AngleVoltage != 0
            ? GetRandomNumberWithPercentageDeviation(phase.AngleVoltage!.Value, 0.05)
            : Math.Abs(GetRandomNumberWithAbsoluteDeviation(phase.AngleVoltage.Value, 0.05));
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

    private static double GetRandomNumberWithAbsoluteDeviation(double value, double deviation)
    {
        var maximum = value + deviation;
        var minimum = value - deviation;
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }

    private static double GetRandomNumberWithPercentageDeviation(double value, double deviation)
    {
        var maximum = value + value * deviation / 100;
        var minimum = value - value * deviation / 100;
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }
}
