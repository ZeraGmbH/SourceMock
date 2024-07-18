using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Possible MT310s2 functions to use.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MT310s2Functions
{
    /// <summary>
    /// Reference meter for EMob.
    /// </summary>
    EMobReferenceMeter = 0,

    /// <summary>
    /// Remote GUI.
    /// </summary>
    RemoteGUI = 1,

    /// <summary>
    /// Reference meter in the DC source for positions 1 to 4.
    /// </summary>
    DCReferenceMeter1 = 2,

    /// <summary>
    /// Reference meter in the DC source for positions 5 to 8.
    /// </summary>
    DCReferenceMeter2 = 3,

    /// <summary>
    /// Calibration of 4 position DC test system.
    /// </summary>
    DCCalibration = 4,
}