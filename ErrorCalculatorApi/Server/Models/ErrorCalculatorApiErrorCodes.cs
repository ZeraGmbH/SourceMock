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
    ERROR_CALCULATOR_NOT_READY,

    /// <summary>
    /// The command has been sent but reported an error.
    /// </summary>
    ERROR_CALCULATOR_COMMAND_FAILED,

    /// <summary>
    /// The command has been sent and the job details report an error.
    /// </summary>
    ERROR_CALCULATOR_JOB_FAILED,

    /// <summary>
    /// Currently the error calculator is not reachable.
    /// </summary>
    ERROR_CALCULATOR_NOT_CONNECTED,

    /// <summary>
    /// Sending data to the error calculator failed.
    /// </summary>
    ERROR_CALCULATOR_SEND_FAILED,
}
