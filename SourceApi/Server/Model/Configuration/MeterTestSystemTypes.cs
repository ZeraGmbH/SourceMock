using System.Text.Json.Serialization;

namespace SourceApi.Model.Configuration;

/// <summary>
/// Supported types of the meter tests system.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MeterTestSystemTypes
{
    /// <summary>
    /// MT786 like.
    /// </summary>
    MT786 = 0,

    /// <summary>
    /// FG30x like.
    /// </summary>
    FG30x = 1,

    /// <summary>
    /// Internal mock implementation for development and testing
    /// purposes.
    /// </summary>
    Mock = 2
}