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
    /// Keyboard, e.g. used for a barcode reader.
    /// </summary>
    Keyboard = 0,
}