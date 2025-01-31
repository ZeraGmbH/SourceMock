using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace MeterTestSystemApi.Services;

/// <summary>
/// All protocols available for a HID.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum HIDProbeProtocols
{
    /// <summary>
    /// Barcode reader.
    /// </summary>
    Barcode = 0,
}