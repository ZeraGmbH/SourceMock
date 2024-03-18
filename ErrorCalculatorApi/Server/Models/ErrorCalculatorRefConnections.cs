using System.Text.Json.Serialization;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// All known reference meter connections of an error calculator.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCalculatorRefConnections
{
    /// <summary>
    /// Reference meter input 1 (e.g. active power)
    /// </summary>
    RefMeter1 = 0,

    /// <summary>
    /// Reference meter input 2 (e.g. reactive power)
    /// </summary>
    RefMeter2 = 1,

    /// <summary>
    /// Reference meter input 3 (e.g. apparent power)
    /// </summary>
    RefMeter3 = 2,
}
