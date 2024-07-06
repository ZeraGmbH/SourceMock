using SharedLibrary.DomainSpecific;
using SourceApi.Model;

namespace SourceApi.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public interface ICapabilitiesMap
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    SourceCapabilities GetCapabilitiesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltageAmplifier"></param>
    /// <param name="currentAmplifier"></param>
    /// <returns></returns>
    public SourceCapabilities GetCapabilitiesByAmplifiers(VoltageAmplifiers voltageAmplifier, CurrentAmplifiers currentAmplifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    public Voltage[] GetVoltageRangesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltageAmplifier"></param>
    /// <returns></returns>
    public Voltage[] GetRangesByAmplifier(VoltageAmplifiers voltageAmplifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    public Current[] GetCurrentRangesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentAmplifier"></param>
    /// <returns></returns>
    public Current[] GetRangesByAmplifier(CurrentAmplifiers currentAmplifier);
}

/// <summary>
/// 
/// </summary>
public class CapabilitiesMap : ICapabilitiesMap
{
    /// <inheritdoc/>
    public SourceCapabilities GetCapabilitiesByModel(string modelName) => GetCapabilitiesByAmplifiers(modelName, modelName);

    /// <inheritdoc/>
    public SourceCapabilities GetCapabilitiesByAmplifiers(VoltageAmplifiers voltageAmplifier, CurrentAmplifiers currentAmplifier) =>
        GetCapabilitiesByAmplifiers(GetVoltageAmplifierKey(voltageAmplifier), GetCurrentAmplifierKey(currentAmplifier));


    private static string GetVoltageAmplifierKey(VoltageAmplifiers amplifier)
    {
        switch (amplifier)
        {
            case VoltageAmplifiers.VU211x0:
            case VoltageAmplifiers.VU211x1:
            case VoltageAmplifiers.VU211x2:
                return "VU211";
            case VoltageAmplifiers.VU221x0:
            case VoltageAmplifiers.VU221x1:
            case VoltageAmplifiers.VU221x2:
            case VoltageAmplifiers.VU221x3:
            case VoltageAmplifiers.VU221x0x2:
            case VoltageAmplifiers.VU221x0x3:
                return "VU221";
            case VoltageAmplifiers.VU220:
            case VoltageAmplifiers.VU220x01:
            case VoltageAmplifiers.VU220x02:
            case VoltageAmplifiers.VU220x03:
            case VoltageAmplifiers.VU220x04:
                return "VU220";
            case VoltageAmplifiers.SVG3020:
                return "SVG3020";
            case VoltageAmplifiers.VUI301:
            case VoltageAmplifiers.VUI302:
                return "VUI302";
            default:
                throw new NotSupportedException($"Unknown voltage amplifier {amplifier}");
        }
    }

    private static string GetCurrentAmplifierKey(CurrentAmplifiers amplifier)
    {
        switch (amplifier)
        {
            case CurrentAmplifiers.VI201x0:
            case CurrentAmplifiers.VI201x0x1:
            case CurrentAmplifiers.VI201x1:
                return "VI201";
            case CurrentAmplifiers.VI202x0:
            case CurrentAmplifiers.VI202x0x1:
            case CurrentAmplifiers.VI202x0x2:
            case CurrentAmplifiers.VI202x0x5:
                return "VI202";
            case CurrentAmplifiers.VI221x0:
                return "VI221";
            case CurrentAmplifiers.VI220:
                return "VI220";
            case CurrentAmplifiers.VI222x0:
            case CurrentAmplifiers.VI222x0x1:
                return "VI222";
            case CurrentAmplifiers.SCG1020:
                return "SCG1020";
            case CurrentAmplifiers.VUI301:
            case CurrentAmplifiers.VUI302:
                return "VUI302";
            default:
                throw new NotSupportedException($"Unknown current amplifier {amplifier}");
        }
    }

    private static SourceCapabilities GetCapabilitiesByAmplifiers(string voltageAmplifier, string currentAmplifier)
    {
        /* See if there are configurations for the amplifiers. */
        if (!VoltageByAmplifier.TryGetValue(voltageAmplifier, out var voltageInfo))
            throw new ArgumentException($"unknown voltage amplifier {voltageAmplifier}", nameof(voltageAmplifier));

        if (!CurrentByAmplifier.TryGetValue(currentAmplifier, out var currentInfo))
            throw new ArgumentException($"unknown current amplifier {currentAmplifier}", nameof(currentAmplifier));

        var (current, _) = currentInfo;
        var (voltage, _) = voltageInfo;

        var capabilties = new SourceCapabilities();

        /* Current configuration uses exactly one phase mapped to three identical configurations. */
        if (current.Phases.Count != 1 || voltage.Phases.Count != 1)
            throw new InvalidOperationException("data mismatch - expected equal number of phases");

        for (var count = 3; count-- > 0;)
            capabilties.Phases.Add(new()
            {
                AcCurrent = current.Phases[0].AcCurrent,
                AcVoltage = voltage.Phases[0].AcVoltage
            });

        /* There may be only one frequency configuration with the same generator mode. */
        if (current.FrequencyRanges.Count != 1 || voltage.FrequencyRanges.Count != 1)
            throw new InvalidOperationException("data mismatch - expected one frequency range");

        if (current.FrequencyRanges[0].Mode != voltage.FrequencyRanges[0].Mode)
            throw new InvalidOperationException("data mismatch - expected same frequency mode");

        capabilties.FrequencyRanges = new();

        capabilties.FrequencyRanges.Add(new()
        {
            Min = current.FrequencyRanges[0].Min.Largest(voltage.FrequencyRanges[0].Min),
            Max = current.FrequencyRanges[0].Max.Smallest(voltage.FrequencyRanges[0].Max),
            Mode = current.FrequencyRanges[0].Mode,
            PrecisionStepSize = current.FrequencyRanges[0].PrecisionStepSize.Largest(voltage.FrequencyRanges[0].PrecisionStepSize),
        });

        return capabilties;
    }

    public Voltage[] GetRangesByAmplifier(VoltageAmplifiers voltageAmplifier) =>
        GetVoltageRangesByModel(GetVoltageAmplifierKey(voltageAmplifier));

    public Current[] GetRangesByAmplifier(CurrentAmplifiers currentAmplifier) =>
        GetCurrentRangesByModel(GetCurrentAmplifierKey(currentAmplifier));

    public Voltage[] GetVoltageRangesByModel(string modelName)
    {
        if (!VoltageByAmplifier.TryGetValue(modelName, out var voltageInfo))
            throw new ArgumentException($"unknown voltage amplifier {modelName}", nameof(modelName));

        return voltageInfo.Item2.Order().ToArray();
    }

    public Current[] GetCurrentRangesByModel(string modelName)
    {
        if (!CurrentByAmplifier.TryGetValue(modelName, out var currentInfo))
            throw new ArgumentException($"unknown current amplifier {modelName}", nameof(modelName));

        return currentInfo.Item2.Order().ToArray();
    }

    private static readonly Dictionary<string, Tuple<SourceCapabilities, Voltage[]>> VoltageByAmplifier = new() {
    { "MT786",  Tuple.Create<SourceCapabilities,Voltage[]>(new () {
        FrequencyRanges = [new(new(45), new(65), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(20), new(500), new(0.001)) }],
    }, [ new(60d), new(125d), new(250d), new(420d) ] )},
    { "VU211", Tuple.Create<SourceCapabilities,Voltage[]>(new () {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(30), new(480), new(0.001)) }],
    }, [ new(480d), new(320d), new(240d), new(160d), new(120d), new(80d), new(60d), new(40d) ]) },
    { "VU220",Tuple.Create<SourceCapabilities,Voltage[]>(new () {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(30), new(320), new(0.001)) }],
    }, [ new(320d), new(160d) ]) },
    { "VU221", Tuple.Create<SourceCapabilities,Voltage[]>(new ()  {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(30), new(320), new(0.001)) }],
    }, [ new(320d), new(160d) ]) },
    { "VUI302", Tuple.Create<SourceCapabilities,Voltage[]>(new ()  {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(30), new(320), new(0.001)) }],
    }, [ new(320d), new(160d), new(80d) ] )},
    { "SVG3020",Tuple.Create<SourceCapabilities,Voltage[]>(new ()  {
        FrequencyRanges = [new(new(15), new(400), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcVoltage = new(new(30), new(500), new(0.001)) }],
    }, [ new(75d), new(150d), new(300d), new(600d) ] )} };

    private static readonly Dictionary<string, Tuple<SourceCapabilities, Current[]>> CurrentByAmplifier = new() {
    { "MT786", Tuple.Create<SourceCapabilities,Current[]>(new () {
        FrequencyRanges = [new(new(45), new(65), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(0.001), new(120), new(0.001)) }],
    }, [new(100d),new(50d),new(20d),new(10d),new(5d),new(2d),new(1d),new(0.5d),new(0.2d),new(0.1d),new(0.05d),new(0.02d) ] )},
    { "VI201",Tuple.Create<SourceCapabilities,Current[]>(new ()  {
        FrequencyRanges = [new(new(15), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(500E-6), new(160), new(0.0001)) }],
    }, [new(160d),new(16d),new(1.6d),new(0.32d),new(0.032d),new(80d),new(8d),new(0.8d),new(0.16d),new(0.016d),new(40d),new(4d),new(0.4d),new(0.08d),new(0.008d),new(20d),new(2d),new(0.2d),new(0.04d),new(0.004d) ] )},
    { "VI202", Tuple.Create<SourceCapabilities,Current[]>(new ()  {
        FrequencyRanges = [new(new(15), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(500E-6), new(120), new(0.0001)) }],
    }, [new(120d),new(16d),new(1.6d),new(0.32d),new(0.032d),new(60d),new(8d),new(0.8d),new(0.16d),new(0.016d),new(30d),new(4d),new(0.4d),new(0.08d),new(0.008d),new(15d),new(2d),new(0.2d),new(0.04d),new(0.004d) ] )},
    { "VI220",Tuple.Create<SourceCapabilities,Current[]>(new ()  {
        FrequencyRanges = [new(new(15), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(500E-6), new(120), new(0.0001)) }],
    }, [new(120d),new(12d),new(1.2d),new(0.12d),new(60d),new(6d),new(0.6d),new(0.06d),new(30d),new(3d),new(0.3d),new(0.03d) ] )},
    { "VI221", Tuple.Create<SourceCapabilities,Current[]>(new ()  {
        FrequencyRanges = [new(new(15), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(500E-6), new(120), new(0.0001)) }],
    }, [new(120d),new(12d),new(1.2d),new(0.12d),new(60d),new(6d),new(0.6d),new(0.06d),new(30d),new(3d),new(0.3d),new(0.03d) ] )},
    { "VI222", Tuple.Create<SourceCapabilities,Current[]>(new () {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(500E-6), new(120), new(0.0001)) }],
    }, [new(120d),new(12d),new(1.2d),new(0.12d),new(60d),new(6d),new(0.6d),new(0.06d),new(30d),new(3d),new(0.3d),new(0.03d) ]) },
    { "VUI302", Tuple.Create<SourceCapabilities,Current[]>(new () {
        FrequencyRanges = [new(new(40), new(70), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(12E-3), new(120), new(0.001)) }],
    }, [new(120d),new(12d),new(1.2d),new(0.12d),new(60d),new(6d),new(0.6d),new(0.06d),new(30d),new(3d),new(0.3d),new(0.03d) ])},
    { "SCG1020", Tuple.Create<SourceCapabilities,Current[]>(new () {
        FrequencyRanges = [new(new(15), new(400), new(0.01), FrequencyMode.SYNTHETIC)],
        Phases = [new() { AcCurrent = new(new(0.0005), new(160), new(0.0001))}],
    }, [new(160d),new(120d),new(80d),new(60d),new(40d),new(30d),new(20d),new(10d),new(5d),new(2d),new(1d),new(0.5d),new(0.2d),new(0.1d),new(0.05d),new(0.025d),new(0.0125d) ] )} };
}
