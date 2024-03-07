using System.Text.Json.Serialization;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Supported communications with an error calculator.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCalculatorProtocols
{
    /// <summary>
    /// Error calculator supported MAD XML communication,
    /// </summary>
    MAD_1 = 0,
}
