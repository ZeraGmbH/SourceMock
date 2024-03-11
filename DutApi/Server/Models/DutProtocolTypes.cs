
using System.Text.Json.Serialization;

namespace DutApi.Models;

/// <summary>
/// All supported protocols to read register values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DutProtocolTypes
{
    /// <summary>
    /// SCPI using a TCP connection.
    /// </summary>
    SCPIOverTCP = 0
}
