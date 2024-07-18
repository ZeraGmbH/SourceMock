using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All possible PCL router types as flags.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NBoxRouterTypes
{
    /// <summary>
    /// NBox Prime.
    /// </summary>
    Prime = 0,

    /// <summary>
    /// NBox G3.
    /// </summary>
    G3 = 1,
}