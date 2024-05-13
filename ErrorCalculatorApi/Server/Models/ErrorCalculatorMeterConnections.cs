using System.Text.Json.Serialization;

namespace ErrorCalculatorApi.Models;

/// <summary>
/// All known connections of an error calculator.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorCalculatorMeterConnections
{
    /// <summary>
    /// No input used
    /// </summary>
    NoInput = 0,

    /// <summary>
    /// Scanning head input, back, e.g. active power
    /// </summary>
    Intern1 = 1,

    /// <summary>
    /// Scanning head input, back, e.g. reactive power
    /// </summary>
    Intern2 = 2,

    /// <summary>
    /// Scanning head input, front, e.g. active power
    /// </summary>
    Extern1 = 3,

    /// <summary>
    /// Scanning head input, front, e.g. reactive power
    /// </summary>
    Extern2 = 4,

    /// <summary>
    /// Front input, BNC
    /// </summary>
    FrontBNC = 5,

    /// <summary>
    /// S0 input 1
    /// </summary>
    S0x1 = 6,

    /// <summary>
    /// S0 input 2
    /// </summary>
    S0x2 = 7,

    /// <summary>
    /// S0 input 3
    /// </summary>
    S0x3 = 8,

    /// <summary>
    /// S0 input 4
    /// </summary>
    S0x4 = 9,

    /// <summary>
    /// S0 input 5
    /// </summary>
    S0x5 = 10,

    /// <summary>
    /// S0 input 6
    /// </summary>
    S0x6 = 11,

    /// <summary>
    /// S0 input 7
    /// </summary>
    S0x7 = 12,

    /// <summary>
    /// S0 input 8
    /// </summary>
    S0x8 = 13,

    /// <summary>
    /// S0 input 9
    /// </summary>
    S0x9 = 14,

    /// <summary>
    /// S0 input 10
    /// </summary>
    S0x10 = 15,

    /// <summary>
    /// S0 input 11
    /// </summary>
    S0x11 = 16,

    /// <summary>
    /// S0 input 12
    /// </summary>
    S0x12 = 17,

    /// <summary>
    /// Input reset button
    /// </summary>
    ResetKey = 18,

    /// <summary>
    /// Software input (eHZ meter mode)
    /// </summary>
    Software = 19,

    /// <summary>
    /// COM server 1, In (UART-1 input)
    /// </summary>
    COM1InUART1 = 20,

    /// <summary>
    /// COM server 1, Out (UART-2 input)
    /// </summary>
    COM1OutUART2 = 21,

    /// <summary>
    /// COM server 2, In (UART-3 input)
    /// </summary>
    COM2InUART3 = 22,

    /// <summary>
    /// COM server 2, Out (UART-4 input)
    /// </summary>
    COM2OutUART4 = 23,

    /// <summary>
    /// Front input, BNC, 1:10
    /// </summary>
    FrontBNC10 = 24,

    /// <summary>
    /// Front input, BNC, 1:100
    /// </summary>
    FrontBNC100 = 25,

    /// <summary>
    /// Front input, BNC, 1:1000
    /// </summary>
    FrontBNC1000 = 26,
}
