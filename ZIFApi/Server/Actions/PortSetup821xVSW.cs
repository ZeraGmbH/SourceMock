using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ZERA.WebSam.Shared;

namespace ZIFApi.Actions;

/// <summary>
/// Reads port configuration from a JSON file.
/// </summary>
public class PortSetup821xVSW : IPortSetup821xVSW
{
    /// <summary>
    /// Define the settings of a single port.
    /// </summary>
    private class RelayDef
    {
        /// <summary>
        /// Index of the phase from 0 to 6.
        /// </summary>
        [JsonPropertyName("PhaseIndex")]
        public int PhaseIndex { get; set; }

        /// <summary>
        /// Name of the phase from A to G matching the index.
        /// </summary>
        [JsonPropertyName("PhaseName")]
        public string PhaseName { get; set; } = null!;

        /// <summary>
        /// Port bit mask - well defined for each port of the 821x.
        /// </summary>
        [JsonPropertyName("BitMask")]
        public string BitMask { get; set; } = null!;

        /// <summary>
        /// Bit settings.
        /// </summary>
        [JsonPropertyName("BitSet")]
        public string BitSet { get; set; } = null!;
    }

    /// <summary>
    /// Setup for a pair of meter form and service type.
    /// </summary>
    private class MeterDef
    {
        /// <summary>
        /// Name of the meter form.
        /// </summary>
        [JsonPropertyName("MeterForm")]
        public string MeterForm { get; set; } = null!;

        /// <summary>
        /// Service definition (currently not used).
        /// </summary>
        [JsonPropertyName("ServiceDefintion")]
        public string ServiceDefintion { get; set; } = null!;

        /// <summary>
        /// Service type to use.
        /// </summary>
        [JsonPropertyName("ServiceType")]
        public string ServiceType { get; set; } = null!;

        /// <summary>
        /// All port setups.
        /// </summary>
        [JsonPropertyName("RelayDefs")]
        public List<RelayDef> RelayDefs { get; set; } = [];
    }

    /// <summary>
    /// Analyse a bye from the definition file.
    /// </summary>
    private static readonly Regex _hexReg = new("^0x([0-9a-f]{1,2})$", RegexOptions.IgnoreCase);

    /// <summary>
    /// For each port a bit set indicates a line connected to the meter.
    /// </summary>
    public static readonly byte[] PortMasks = [
        0x0c,
        0x3f,
        0x3f,
        0x9f,
        0xff,
        0xe0,
        0x00
    ];

    /// <summary>
    /// All setups from the definition file.
    /// </summary>
    private static readonly Dictionary<string, string> _typeMappings = new(){
        { "1PH-3W", "1PH 3W" },
        { "1PH", "1PH" },
        { "3W-DELTA", "3WD" },
        { "3W-NETWORK", "3WN" },
        { "4W-DELTA A HI", "4WD A" },
        { "4W-DELTA C HI", "4WD" },
        { "4W-DELTA HI", "4WD" },
        { "4W-DELTA", "4WD" },
        { "4W-WYE (no N current)", "4WY" },
        { "4W-WYE", "4WY" },
        { "NOT USED", "" },
    };

    /// <summary>
    /// Port setups for all pairs of meter form and service type. Available
    /// after the configuration file has been loaded.
    /// </summary>
    private readonly Dictionary<PortKey, byte[]> _portSetups = [];

    /// <inheritdoc/>
    public Task<Dictionary<PortKey, byte[]>> PortSetups { get; private set; }

    /// <summary>
    /// Get a byte from a string.
    /// </summary>
    /// <param name="value">String representation of the byte.</param>
    /// <returns>Byte value.</returns>
    private static byte DecodeByte(string value)
    {
        var match = _hexReg.Match(value);

        if (!match.Success) throw new ArgumentException("bad byte");

        return byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
    }

    /// <summary>
    /// Load the JSON configuration and fill the map.
    /// </summary>
    private async Task LoadAsync()
    {
        /* Configuration file will be copied to output directory. */
        var path = Path.Combine(
            Path.GetDirectoryName(typeof(PortSetup821xVSW).Assembly.Location)!,
            "821x_VSW_Setups.json"
        );

        /* Load the raw JSON contents. */
        var json = await File.ReadAllBytesAsync(path);

        /* Parse to definitions. */
        var forms = JsonSerializer.Deserialize<MeterDef[]>(json, LibUtils.JsonSettings)!;

        /* Create map. */
        foreach (var form in forms)
        {
            /* Some validation. */
            if (string.IsNullOrEmpty(form.MeterForm))
                throw new ArgumentException("no meter form");
            if (string.IsNullOrEmpty(form.ServiceType))
                throw new ArgumentException($"no service type for {form.MeterForm}");

            /* Map the service type as used internally. */
            var serviceType = _typeMappings[form.ServiceType];
            var key = new PortKey(form.MeterForm, serviceType);

            /* Create the mask field to fill. */
            var phaseSets = new byte[7];

            /* Process all relays. */
            var indexes = new HashSet<int>();

            foreach (var relay in form.RelayDefs)
            {
                /* Validate index. */
                if (relay.PhaseIndex < 0 || relay.PhaseIndex > phaseSets.Length)
                    throw new ArgumentException($"{key}: phase index {relay.PhaseIndex} must be between 0 and {phaseSets.Length}");

                if (relay.PhaseName.Length != 1 || relay.PhaseIndex != relay.PhaseName[0] - 'A')
                    throw new ArgumentException($"{key}: phase index {relay.PhaseIndex} does not match phase name {relay.PhaseName}");

                if (!indexes.Add(relay.PhaseIndex))
                    throw new ArgumentException($"{key}: phase {relay.PhaseIndex} declared multiple times");

                /* Decode bit mask. */
                var mask = DecodeByte(relay.BitMask);
                var set = DecodeByte(relay.BitSet);

                /* Validate mask. */
                if (mask != PortMasks[relay.PhaseIndex])
                    throw new ArgumentException($"{key}: unexpected phase bit mask {relay.BitMask} for phase {relay.PhaseIndex}");
                if ((set & ~mask) != 0)
                    throw new ArgumentException($"{key}: try to set bits {relay.BitSet} on phase {relay.PhaseIndex} not covered by mask {relay.BitMask}");

                /* Remember. */
                phaseSets[relay.PhaseIndex] = set;
            }

            if (indexes.Count != 7)
                throw new ArgumentException($"{key}: not all phases defined");

            /* Try to add. */
            if (!string.IsNullOrEmpty(serviceType))
                if (!_portSetups.TryGetValue(key, out var existing))
                    _portSetups.Add(key, phaseSets);
                else
                {
                    /* May have different service definitions but port configuration should be the same. */
                    for (var i = Math.Max(phaseSets.Length, existing.Length); i-- > 0;)
                        if (phaseSets[i] != existing[i])
                            throw new ArgumentException($"duplicate phase set for {key}");
                }
        }
    }

    /// <summary>
    /// Initialize the loader.
    /// </summary>
    public PortSetup821xVSW()
    {
        PortSetups = LoadAsync().ContinueWith(t => _portSetups, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);
    }
}