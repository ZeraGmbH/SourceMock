using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// All supported reference meters.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferenceMeters
{
    /// <summary>
    /// EPZ303-1
    /// </summary>
    EPZ303x1 = 0,

    /// <summary>
    /// EPZ303-5
    /// </summary>
    EPZ303x5 = 1,

    /// <summary>
    /// EPZ103
    /// </summary>
    EPZ103 = 2,

    /// <summary>
    /// RMM303-6
    /// </summary>
    RMM303x6 = 3,

    /// <summary>
    /// RMM303-8
    /// </summary>
    RMM303x8 = 4,

    /// <summary>
    /// COM3000
    /// </summary>
    COM3000 = 5,

    /// <summary>
    /// COM3003
    /// </summary>
    COM3003 = 6,

    /// <summary>
    /// COM3003-DC
    /// </summary>
    COM3003xDC = 7,

    /// <summary>
    /// EPZ303-8
    /// </summary>
    EPZ303x8 = 8,

    /// <summary>
    /// EPZ303-9
    /// </summary>
    EPZ303x9 = 9,

    /// <summary>
    /// EPZ303-8-1
    /// </summary>
    EPZ303x8x1 = 10,

    /// <summary>
    /// RMM3000-1
    /// </summary>
    RMM3000x1 = 11,

    /// <summary>
    /// COM3003-DC-2-1
    /// </summary>
    COM3003xDCx2x1 = 12,

    /// <summary>
    /// COM5003-1
    /// </summary>
    COM5003x1 = 13,

    /// <summary>
    /// EPZ350-00
    /// </summary>
    EPZ350x00 = 14,

    /// <summary>
    /// COM5003-0-1
    /// </summary>
    COM5003x0x1 = 15,

    /// <summary>
    /// EPZ303-10
    /// </summary>
    EPZ303x10 = 16,

    /// <summary>
    /// EPZ303-10-1
    /// </summary>
    EPZ303x10x1 = 17,

    /// <summary>
    /// COM3003-1-2
    /// </summary>
    COM3003x1x2 = 18,

    /// <summary>
    /// MT310s2
    /// </summary>
    MT310s2 = 19,
}
