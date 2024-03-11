using System.Text.Json.Serialization;

namespace DutApi.Models;

/// <summary>
/// Register types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DutRegisterTypes
{
    /// <summary>
    /// Active power, quadrants 1 and 4.
    /// </summary>
    PPlus = 0,

    /// <summary>
    /// Active power, quadrants 2 and 3.
    /// </summary>
    PMinus = 1,

    /// <summary>
    /// Active power, quadrant 1.
    /// </summary>
    P1 = 2,

    /// <summary>
    /// Active power, quadrant 2.
    /// </summary>
    P2 = 3,

    /// <summary>
    /// Active power, quadrant 3.
    /// </summary>
    P3 = 4,

    /// <summary>
    /// Active power, quadrant 4.
    /// </summary>
    P4 = 5,

    /// <summary>
    /// Reactive power, quadrants 1 and 2.
    /// </summary>
    QPlus = 6,

    /// <summary>
    /// Reactive power, quadrants 3 and 4.
    /// </summary>
    QMinus = 7,

    /// <summary>
    /// Reactive power, quadrant 1.
    /// </summary>
    Q1 = 8,

    /// <summary>
    /// Reactive power, quadrant 2.
    /// </summary>
    Q2 = 9,

    /// <summary>
    /// Reactive power, quadrant 3.
    /// </summary>
    Q3 = 10,

    /// <summary>
    /// Reactive power, quadrant 4.
    /// </summary>
    Q4 = 11,

    /// <summary>
    /// Apparent power.
    /// </summary>
    S = 12,
}
