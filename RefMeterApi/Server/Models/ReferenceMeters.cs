using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferenceMeters
{
    /// <summary>
    /// 
    /// </summary>
    EPZ303x1,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x5,

    /// <summary>
    /// 
    /// </summary>
    EPZ103,

    /// <summary>
    /// 
    /// </summary>
    RMM303x6,

    /// <summary>
    /// 
    /// </summary>
    RMM303x8,

    /// <summary>
    /// 
    /// </summary>
    COM3000,

    /// <summary>
    /// 
    /// </summary>
    COM3003,

    /// <summary>
    /// 
    /// </summary>
    COM3003xDC,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x8,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x9,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x8x1,

    /// <summary>
    /// 
    /// </summary>
    RMM3000x1,

    /// <summary>
    /// 
    /// </summary>
    COM3003xDCx2x1,

    /// <summary>
    /// 
    /// </summary>
    COM5003x1,

    /// <summary>
    /// 
    /// </summary>
    EPZ350x00,

    /// <summary>
    /// 
    /// </summary>
    COM5003x0x1,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x10,

    /// <summary>
    /// 
    /// </summary>
    EPZ303x10x1,

    /// <summary>
    /// 
    /// </summary>
    COM3003x1x2,

    /// <summary>
    /// 
    /// </summary>
    MT310s2,
}
