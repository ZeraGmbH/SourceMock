using System.Runtime.CompilerServices;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.Models.Source;

namespace ZeraDevices.MeterTestSystem.FG30x;

/// <summary>
/// Mapping of amplifier and reference meter types to the
/// code used in the serial API of the frequency generators.
/// </summary>
public static class CodeMappings
{
    /// <summary>
    /// Provides the mapping for current amplifiers to API codes.
    /// </summary>
    public static readonly Dictionary<CurrentAmplifiers, int> Current = new()
        {
            { CurrentAmplifiers.LABSMP715, 46 },
            { CurrentAmplifiers.SCG1000x00, 46 },
            { CurrentAmplifiers.SCG1020, 42 },
            { CurrentAmplifiers.SCG750x00, 46 },
            { CurrentAmplifiers.V200, 21 },
            { CurrentAmplifiers.V200x2, 34 },
            { CurrentAmplifiers.V200x4, 40 },
            { CurrentAmplifiers.VI201x0, 35 },
            { CurrentAmplifiers.VI201x0x1, 39 },
            { CurrentAmplifiers.VI201x1, 35 },
            { CurrentAmplifiers.VI202x0, 36 },
            { CurrentAmplifiers.VI202x0x1, 41 },
            { CurrentAmplifiers.VI202x0x2, 45 },
            { CurrentAmplifiers.VI202x0x5, 47 },
            { CurrentAmplifiers.VI220, 22 },
            { CurrentAmplifiers.VI221x0, 37 },
            { CurrentAmplifiers.VI222x0, 38 },
            { CurrentAmplifiers.VI222x0x1, 46 },
            { CurrentAmplifiers.VUI301, 23 },
            { CurrentAmplifiers.VUI302, 23 },
        };

    /// <summary>
    /// Provides the mapping for auxiliary current amplifiers to API codes.
    /// </summary>
    public static readonly Dictionary<CurrentAuxiliaries, int> AuxCurrent = new()
        {
            { CurrentAuxiliaries.V200, 21 },
            { CurrentAuxiliaries.V200x2, 34 },
            { CurrentAuxiliaries.VI200x4, 40 },
            { CurrentAuxiliaries.VI201x0, 35 },
            { CurrentAuxiliaries.VI201x0x1, 39 },
            { CurrentAuxiliaries.VI201x1, 35 },
            { CurrentAuxiliaries.VI202x0, 36 },
            { CurrentAuxiliaries.VI220, 22 },
            { CurrentAuxiliaries.VI221x0, 37 },
            { CurrentAuxiliaries.VI222x0, 38 },
            { CurrentAuxiliaries.VUI301, 23 },
            { CurrentAuxiliaries.VUI302, 23 },
        };

    /// <summary>
    /// Provides the mapping for voltage amplifiers to API codes.
    /// </summary>
    public static readonly Dictionary<VoltageAmplifiers, int> Voltage = new()
        {
            { VoltageAmplifiers.LABSMP21200, 36 },
            { VoltageAmplifiers.SVG1200x00, 36 },
            { VoltageAmplifiers.SVG3020, 37 },
            { VoltageAmplifiers.V210, 1 },
            { VoltageAmplifiers.VU211x0, 30 },
            { VoltageAmplifiers.VU211x1, 30 },
            { VoltageAmplifiers.VU211x2, 30 },
            { VoltageAmplifiers.VU220, 2 },
            { VoltageAmplifiers.VU220x01, 18 },
            { VoltageAmplifiers.VU220x02, 17 },
            { VoltageAmplifiers.VU220x03, 19 },
            { VoltageAmplifiers.VU220x04, 20 },
            { VoltageAmplifiers.VU221x0, 31 },
            { VoltageAmplifiers.VU221x0x2, 36 },
            { VoltageAmplifiers.VU221x0x3, 38 },
            { VoltageAmplifiers.VU221x1, 32 },
            { VoltageAmplifiers.VU221x2, 33 },
            { VoltageAmplifiers.VU221x3, 32 },
            { VoltageAmplifiers.VUI301, 3 },
            { VoltageAmplifiers.VUI302, 3 },
        };

    /// <summary>
    /// Provides the mapping for auxiliary voltage amplifiers to API codes.
    /// </summary>
    public static readonly Dictionary<VoltageAuxiliaries, int> AuxVoltage = new()
        {
            { VoltageAuxiliaries.SVG150x00, 18 },
            { VoltageAuxiliaries.V210, 1 },
            { VoltageAuxiliaries.VU211x0, 30 },
            { VoltageAuxiliaries.VU211x1, 30 },
            { VoltageAuxiliaries.VU211x2, 30 },
            { VoltageAuxiliaries.VU220, 2 },
            { VoltageAuxiliaries.VU220x01, 18 },
            { VoltageAuxiliaries.VU220x02, 17 },
            { VoltageAuxiliaries.VU220x03, 19 },
            { VoltageAuxiliaries.VU220x04, 20 },
            { VoltageAuxiliaries.VU220x6, 18 },
            { VoltageAuxiliaries.VU221x0, 31 },
            { VoltageAuxiliaries.VU221x1, 32 },
            { VoltageAuxiliaries.VU221x2, 33 },
            { VoltageAuxiliaries.VU221x3, 32 },
            { VoltageAuxiliaries.VUI301, 3 },
            { VoltageAuxiliaries.VUI302, 3 },
        };

    /// <summary>
    /// Provides the mapping for reference meters to API codes.
    /// </summary>
    public static readonly Dictionary<ReferenceMeters, int> RefMeter = new()
    {
        { ReferenceMeters.COM3000, 44 },
        { ReferenceMeters.COM3003, 50 },
        { ReferenceMeters.COM3003x1x2, 54 },
        { ReferenceMeters.COM3003xDC, 51 },
        { ReferenceMeters.COM3003xDCx2x1, 61 },
        { ReferenceMeters.COM5003x0x1, 71 },
        { ReferenceMeters.COM5003x1, 65 },
        { ReferenceMeters.EPZ103, 47 },
        { ReferenceMeters.EPZ303x1, 41 },
        { ReferenceMeters.EPZ303x10, 70 },
        { ReferenceMeters.EPZ303x10x1, 72 },
        { ReferenceMeters.EPZ303x5, 42 },
        { ReferenceMeters.EPZ303x8, 62 },
        { ReferenceMeters.EPZ303x8x1, 64 },
        { ReferenceMeters.EPZ303x9, 69 },
        { ReferenceMeters.EPZ350x00, 66 },
        { ReferenceMeters.MT310s2, 73 },
        { ReferenceMeters.RMM3000x1, 63 },
        { ReferenceMeters.RMM303x6, 48 },
        { ReferenceMeters.RMM303x8, 49 },
    };

    private static void Dump<T1, T2>(Dictionary<T1, int> mapping, Dictionary<T2, int> auxMapping, [CallerArgumentExpression(nameof(mapping))] string? name = null) where T1 : notnull where T2 : notnull
    {
        Console.WriteLine($"Mapping for {name}");

        var all = mapping
            .Select(e => new { Key = $"{e.Key.GetType().Name}:{e.Key}", e.Value })
            .Concat(auxMapping.Select(e => new { Key = $"{e.Key.GetType().Name}:{e.Key}", e.Value }))
            .GroupBy(e => e.Value, e => e.Key)
            .OrderBy(g => g.Key);

        foreach (var group in all)
            Console.WriteLine($"{group.Key}: {string.Join(", ", group)}");

        Console.WriteLine();
    }

    private static void Dump<T>(Dictionary<T, int> mapping, [CallerArgumentExpression(nameof(mapping))] string? name = null) where T : notnull
    {
        Dump(mapping, new Dictionary<T, int>(), name);
    }

    /// <summary>
    /// Report the current mapping to the console - just for checkup.
    /// </summary>
    public static void Dump()
    {
        Dump(Voltage, AuxVoltage);
        Dump(Current, AuxCurrent);
        Dump(RefMeter);
    }
}