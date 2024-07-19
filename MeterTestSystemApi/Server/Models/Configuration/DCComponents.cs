using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Various connections for a DC test system.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DCComponents
{
    /// <summary>
    /// SCG8-00V00DC current amplifier.
    /// </summary>
    CurrentSCG8 = 0,

    /// <summary>
    /// SCG80-00V00DC current amplifier.
    /// </summary>
    CurrentSCG80 = 1,

    /// <summary>
    /// SCG750-00V00DC current amplifier.
    /// </summary>
    CurrentSCG750 = 2,

    /// <summary>
    /// SCG06-00V00DC current amplifier.
    /// </summary>
    CurrentSCG06 = 3,

    /// <summary>
    /// SCG1000-00V00DC current amplifier.
    /// </summary>
    CurrentSCG1000 = 4,

    /// <summary>
    /// SVG1200-00V00DC voltage amplifier.
    /// </summary>
    VoltageSVG1200 = 5,

    /// <summary>
    /// SVG150-00V00DC auxiliary voltage amplifier.
    /// </summary>
    VoltageSVG150 = 6,

    /// <summary>
    /// Test system control.
    /// </summary>
    SPS = 7,

    /// <summary>
    /// DC U/I generator control with FG middleware integrated in the source.
    /// </summary>
    FGControl = 8,
}