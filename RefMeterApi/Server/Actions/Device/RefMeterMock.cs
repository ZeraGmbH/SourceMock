using RefMeterApi.Models;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public partial class RefMeterMock : IRefMeter
{

    private MeasurementModes? _measurementMode = null;
    private readonly ISource _source;

    /// <summary>
    /// 
    /// </summary>
    public RefMeterMock(ISource source)
    {
        _source = source;
    }
    /// <summary>
    /// 
    /// </summary>
    public bool Available => true;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        return Task.FromResult(_measurementMode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeasureOutput> GetActualValues()
    {
        Random r = new Random();
        var loadpoint = _source.GetCurrentLoadpoint() ?? new Loadpoint()
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
        var activePower = loadpoint.Phases[0].Current.Rms * loadpoint.Phases[0].Voltage.Rms * Math.Cos(Math.Abs(loadpoint.Phases[0].Current.Angle - loadpoint.Phases[0].Voltage.Angle));
        var reactivePower = loadpoint.Phases[0].Current.Rms * loadpoint.Phases[0].Voltage.Rms * Math.Sin(Math.Abs(loadpoint.Phases[0].Current.Angle - loadpoint.Phases[0].Voltage.Angle));
        var apparentPower = Math.Sqrt(activePower * activePower + reactivePower * reactivePower);

        var measureOutputPhases = new List<MeasureOutputPhase>();
        foreach (var phase in loadpoint.Phases)
        {
            var measureOutputPhase = new MeasureOutputPhase()
            {
                Current = Math.Abs(GetRandomNumber(phase.Current.Rms, 0.02)),
                AngleCurrent = GetRandomNumber(phase.Current.Angle, 0.02),
                Voltage = Math.Abs(GetRandomNumber(phase.Voltage.Rms, 0.02)),
                AngleVoltage = Math.Abs(GetRandomNumber(phase.Voltage.Angle, 0.02)),
                // P=U*I*cos(phi) phi -> Angle Current - Angle Voltage
                ActivePower = GetRandomNumber(phase.Current.Rms * phase.Voltage.Rms * Math.Cos(Math.Abs(phase.Current.Angle - phase.Voltage.Angle)), 0.02),
                // Q=U*I*sin(phi) phi -> Angle Current - Angle Voltage
                ReactivePower = GetRandomNumber(reactivePower, 0.02),
                // S= Wurzel(P^2 + Q^2)
                ApparentPower = GetRandomNumber(apparentPower, 0.02),
                // Lambda = P/Q
                PowerFactor = apparentPower != 0 ? GetRandomNumber(activePower / apparentPower, 0.02) : 0,
            };
            measureOutputPhases.Add(measureOutputPhase);
        }

        MeasureOutput measureOutput = new()
        {
            Frequency = GetRandomNumber(loadpoint.Frequency.Value, 0.02),
            PhaseOrder = "123",
            Phases = measureOutputPhases
        };

        return Task.FromResult(measureOutput);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        return Task.FromResult((MeasurementModes[])Enum.GetValues(typeof(MeasurementModes)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode"></param>
    /// <returns>Must return something - no task requeired without device</returns>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        _measurementMode = mode;
        return Task.FromResult<MeasurementModes>(mode);
    }

    private static double GetRandomNumber(double value, double deviation)
    {
        var maximum = value + deviation;
        var minimum = value - deviation;
        Random random = new Random();
        return random.NextDouble() * (maximum - minimum) + minimum;
    }
}
