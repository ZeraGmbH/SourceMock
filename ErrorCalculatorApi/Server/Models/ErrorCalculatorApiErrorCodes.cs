using System.Text.Json.Serialization;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// Lists all specific errors that may accur in the ErrorCalculatorApi
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCalculatorApiErrorCodes
{
    /// <summary>
    /// Generated when a frequency generator connected error calculator
    /// should be used prior to configuring the meter test system itself.
    /// </summary>
    ERROR_CALCULATOR_NOT_READY
}
