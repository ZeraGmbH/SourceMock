using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All possible PCL router types as flags.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter)), Flags]
public enum NBoxRouterTypes
{
    /// <summary>
    /// No type choosen.
    /// </summary>
    None = 0,

    /// <summary>
    /// NBox Prime.
    /// </summary>
    Prime = 0x01,

    /// <summary>
    /// NBox G3.
    /// </summary>
    G3 = 0x02,

    /// <summary>
    /// All types.
    /// </summary>
    All = Prime | G3
}