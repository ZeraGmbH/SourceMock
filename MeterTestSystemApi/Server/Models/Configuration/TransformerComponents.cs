using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All Components provided for a instrument transformer.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransformerComponents
{
    /// <summary>
    /// SPS (ILC171) tests system control.
    /// </summary>
    SPS = 0,

    /// <summary>
    /// STR 260 control WPE/RPE phase 1.
    /// </summary>
    STR260Phase1 = 1,

    /// <summary>
    /// STR 260 control WPE/RPE phase 2.
    /// </summary>
    STR260Phase2 = 2,

    /// <summary>
    /// STR 260 control WPE/RPE phase 3.
    /// </summary>
    STR260Phase3 = 3,

    /// <summary>
    /// WM3000 I or WM1000 I current transformer measuring system.
    /// </summary>
    CurrentWM3000or1000 = 4,

    /// <summary>
    /// WM3000 U or WM1000 U voltage transformer measuring system.
    /// </summary>
    VoltageWM3000or1000 = 5,
}