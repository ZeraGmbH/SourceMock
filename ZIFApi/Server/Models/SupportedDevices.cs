using System.Text.Json.Serialization;

namespace ZIFApi.Models;

/// <summary>
/// List of all known ZIF devices.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SupportedDevices
{
    /// <summary>
    /// PowerMaster Model 8121 Single Position Test Board.
    /// </summary>
    PowerMaster8121 = 0
}