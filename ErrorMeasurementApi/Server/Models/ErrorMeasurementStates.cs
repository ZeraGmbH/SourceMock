using System.Text.Json.Serialization;

namespace ErrorMeasurementApi.Models;

/// <summary>
/// Possible operation states of an error measurement.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorMeasurementStates
{
    /// <summary>
    /// The error measurement can be used.
    /// </summary>
    Active,

    /// <summary>
    /// The last error measurement has been finished.
    /// </summary>
    Finished,

    /// <summary>
    /// The error measurement is not available.
    /// </summary>
    NotActive,

    /// <summary>
    /// An error measurement is active.
    /// </summary>
    Running,
}
