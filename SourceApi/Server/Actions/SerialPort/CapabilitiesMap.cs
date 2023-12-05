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
        if (!VoltageByAmplifier.TryGetValue(voltageAmplifier, out var voltage))
            throw new ArgumentException(nameof(voltageAmplifier));

        if (!CurrentByAmplifier.TryGetValue(currentAmplifier, out var current))
            throw new ArgumentException(nameof(currentAmplifier));

        var capabilties = new SourceCapabilities();

        /* Current configuration uses exactly one phase mapped to three identical configurations. */
        if (current.Phases.Count != 1 || voltage.Phases.Count != 1)
            throw new InvalidOperationException("data mismatch - expected equal number of phases");

        for (var count = 3; count-- > 0;)
            capabilties.Phases.Add(new()
            {
                Current = current.Phases[0].Current,
                Voltage = voltage.Phases[0].Voltage
            });

        /* There may be only one frequency configuration with the same generator mode. */
        if (current.FrequencyRanges.Count != 1 || voltage.FrequencyRanges.Count != 1)
            throw new InvalidOperationException("data mismatch - expected one frequency range");

        if (current.FrequencyRanges[0].Mode != voltage.FrequencyRanges[0].Mode)
            throw new InvalidOperationException("data mismatch - expected same frequency mode");

        capabilties.FrequencyRanges.Add(new()
        {
            Min = Math.Max(current.FrequencyRanges[0].Min, voltage.FrequencyRanges[0].Min),
            Max = Math.Min(current.FrequencyRanges[0].Max, voltage.FrequencyRanges[0].Max),
            Mode = current.FrequencyRanges[0].Mode,
            PrecisionStepSize = Math.Max(current.FrequencyRanges[0].Min, voltage.FrequencyRanges[0].Max),
        });

        return capabilties;
    }

    private static readonly Dictionary<string, SourceCapabilities> VoltageByAmplifier = new() {
    { "MT786", new() {
        FrequencyRanges = new() { new(45, 65, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Voltage = new(20, 500, 0.001) } },
    } },
    { "VU211", new () {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new () { new() { Voltage = new(30, 480, 0.001) } },
    } },
    { "VU220", new () {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new () { new() { Voltage = new(30, 320, 0.001) } },
    } },
    { "VU221", new() {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Voltage = new(30, 320, 0.001) } },
    } },
    { "VUI302", new() {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Voltage = new(30, 320, 0.001) } },
    } } };

    private static readonly Dictionary<string, SourceCapabilities> CurrentByAmplifier = new() {
    { "MT786", new() {
        FrequencyRanges = new() { new(45, 65, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(0.001, 120, 0.001) } },
    } },
    { "VI201", new() {
        FrequencyRanges = new() { new(15, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(500E-6, 160, 0.001) } },
    } },
    { "VI202", new() {
        FrequencyRanges = new() { new(15, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(500E-6, 120, 0.001) } },
    } },
    { "VI220", new() {
        FrequencyRanges = new() { new(15, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(500E-6, 120, 0.001) } },
    } },
    { "VI221", new() {
        FrequencyRanges = new() { new(15, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(500E-6, 120, 0.001) } },
    } },
    { "VI222", new() {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(500E-6, 120, 0.001) } },
    } },
    { "VUI302", new() {
        FrequencyRanges = new() { new(40, 70, 0.01, FrequencyMode.SYNTHETIC) },
        Phases = new() { new() { Current = new(12E-3, 120, 0.001) } },
    } } };
}
