using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// Possible operation states of an error measurement.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorMeasurementStates
{
    /// <summary>
    /// 
    /// </summary>
    Active,

    /// <summary>
    /// 
    /// </summary>
    Finished,

    /// <summary>
    /// 
    /// </summary>
    NotActive,

    /// <summary>
    /// 
    /// </summary>
    Run,
}
