using System.Text.Json.Serialization;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Hot to connect to the error calculator.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCalculatorConnectionTypes
{
    /// <summary>
    /// Use a TCP/IP endpoint with a given port.
    /// </summary>
    TCP = 0,
}
