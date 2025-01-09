using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Result of a probe operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProbeResult
{
    /// <summary>
    /// Planned to probe.
    /// </summary>
    Planned = 0,

    /// <summary>
    /// Probing successful.
    /// </summary>
    Succeeded = 1,

    /// <summary>
    /// Probing failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// Probing skipped - no result.
    /// </summary>
    Skipped = 3
}

