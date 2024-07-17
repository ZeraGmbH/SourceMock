using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All Components provided for a instrument transformer.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter)), Flags]
public enum TransformerComponents
{
    /// <summary>
    /// No component choosen.
    /// </summary>
    None = 0,

    /// <summary>
    /// SPS (ILC171) tests system control.
    /// </summary>
    SPS = 0x01,

    /// <summary>
    /// STR 260 control WPE/RPE phase 1.
    /// </summary>
    STR260Phase1 = 0x02,

    /// <summary>
    /// STR 260 control WPE/RPE phase 2.
    /// </summary>
    STR260Phase2 = 0x04,

    /// <summary>
    /// STR 260 control WPE/RPE phase 3.
    /// </summary>
    STR260Phase3 = 0x08,

    /// <summary>
    /// WM3000 I or WM1000 I current transformer measuring system.
    /// </summary>
    CurrentWM3000or1000 = 0x10,

    /// <summary>
    /// WM3000 U or WM1000 U voltage transformer measuring system.
    /// </summary>
    VoltageWM3000or1000 = 0x20,

    /// <summary>
    /// All components.
    /// </summary>
    All =
        SPS |
        STR260Phase1 |
        STR260Phase2 |
        STR260Phase3 |
        CurrentWM3000or1000 |
        VoltageWM3000or1000,
}