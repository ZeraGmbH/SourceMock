using System.Text.Json.Serialization;

namespace DutApi.Models;

/// <summary>
/// Register types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DutRegisterUnits
{
    /// <summary>
    /// Energy with base unit Wh. 
    /// </summary>
    Wh = 0,

    /// <summary>
    /// Power with base unit W.
    /// </summary>
    W = 1,

    /// <summary>
    /// Apparent power with base unit VA.
    /// </summary>
    VA = 2,

    /// <summary>
    /// Reactive ower with base unit VAR.
    /// </summary>
    VAR = 3
}
