using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Various connections for a DC test system - combined using bit fields.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter)), Flags]
public enum DCComponents
{
    /// <summary>
    /// No component choosen.
    /// </summary>
    None = 0,

    /// <summary>
    /// SCG8-00V00DC current amplifier.
    /// </summary>
    CurrentSCG8 = 0x001,

    /// <summary>
    /// SCG80-00V00DC current amplifier.
    /// </summary>
    CurrentSCG80 = 0x002,

    /// <summary>
    /// SCG750-00V00DC current amplifier.
    /// </summary>
    CurrentSCG750 = 0x004,

    /// <summary>
    /// SCG06-00V00DC current amplifier.
    /// </summary>
    CurrentSCG06 = 0x008,

    /// <summary>
    /// SCG1000-00V00DC current amplifier.
    /// </summary>
    CurrentSCG1000 = 0x010,

    /// <summary>
    /// SVG1200-00V00DC voltage amplifier.
    /// </summary>
    VoltageSVG1200 = 0x020,

    /// <summary>
    /// SVG150-00V00DC auxiliary voltage amplifier.
    /// </summary>
    VoltageSVG150 = 0x040,

    /// <summary>
    /// Test system control.
    /// </summary>
    SPS = 0x080,

    /// <summary>
    /// DC U/I generator control with FG middleware integrated in the source.
    /// </summary>
    FGControl = 0x100,
}