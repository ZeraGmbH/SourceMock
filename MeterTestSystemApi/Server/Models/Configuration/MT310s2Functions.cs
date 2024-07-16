using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Possible MT310s2 functions to use.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter)), Flags]
public enum MT310s2Functions
{
    /// <summary>
    /// No function selected.
    /// </summary>
    None = 0,

    /// <summary>
    /// Reference meter for EMob.
    /// </summary>
    EMobReferenceMeter = 0x01,

    /// <summary>
    /// Remote GUI.
    /// </summary>
    RemoteGUI = 0x02,

    /// <summary>
    /// Reference meter in the DC source for positions 1 to 4.
    /// </summary>
    DCReferenceMeter1 = 0x04,

    /// <summary>
    /// Reference meter in the DC source for positions 5 to 8.
    /// </summary>
    DCReferenceMeter2 = 0x08,

    /// <summary>
    /// Calibration of 4 position DC test system.
    /// </summary>
    DCCalibration = 0x10,
}