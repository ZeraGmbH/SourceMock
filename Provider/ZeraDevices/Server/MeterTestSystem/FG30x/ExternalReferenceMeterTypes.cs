using System.Text.Json.Serialization;

namespace ZeraDevices.MeterTestSystem.FG30x;

/// <summary>
/// All external reference meters.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExternalReferenceMeterTypes
{
    /// <summary>
    /// MT3000 series - using MT786 protocol
    /// </summary>
    MT3000 = 0
}