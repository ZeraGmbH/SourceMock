using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// All error codes produced by RefMeterApi
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RefMeterApiErrorCodes
{
    /// <summary>
    /// Indicates that the reference meter used is not yet configured.
    /// </summary>
    REF_METER_NOT_READY,
}
