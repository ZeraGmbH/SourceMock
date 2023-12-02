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
    COM3003 = 50,
    /// <summary>
    /// 
    /// </summary>
    COM3003x1x2 = 54,
    /// <summary>
    /// 
    /// </summary>
    COM3003x1x3 = 55,
    /// <summary>
    /// 
    /// </summary>
    COM3003DC = 51,
    /// <summary>
    /// 
    /// </summary>
    COM3003DCx1x2 = 61,
    /// <summary>
    /// 
    /// </summary>
    COM303x1 = 43,
    /// <summary>
    /// 
    /// </summary>
    COM303x2 = 45,
    /// <summary>
    /// 
    /// </summary>
    COM303x3 = 44,
    /// <summary>
    /// 
    /// </summary>
    COM303x3x1 = 57,
    /// <summary>
    /// 
    /// </summary>
    COM5003x1 = 65,
    /// <summary>
    /// 
    /// </summary>
    COM5003x1x1 = 71,
    /// <summary>
    /// 
    /// </summary>
    EPZ103x1 = 47,
    /// <summary>
    /// 
    /// </summary>
    EPZ103x1x2 = 60,
    /// <summary>
    /// 
    /// </summary>
    EPZ103x3x1 = 68,
    /// <summary>
    /// 
    /// </summary>
    EPZ301x119x3 = 52,
    /// <summary>
    /// 
    /// </summary>
    EPZ301x119x3x2 = 53,
    /// <summary>
    /// 
    /// </summary>
    EPZ303 = 46,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x1 = 41,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x10 = 70,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x10x1 = 72,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x5 = 42,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x5x1 = 56,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x5x2 = 58,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x6 = 59,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x8 = 62,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x8x1 = 64,
    /// <summary>
    /// 
    /// </summary>
    EPZ303x9 = 69,
    /// <summary>
    /// 
    /// </summary>
    EPZ350x0 = 66,
    /// <summary>
    /// 
    /// </summary>
    EPZ350x1 = 67,
    /// <summary>
    /// 
    /// </summary>
    RMM3000x1 = 63,
    /// <summary>
    /// 
    /// </summary>
    RMM303x6 = 48,
    /// <summary>
    /// 
    /// </summary>
    RMM303x8 = 49,
}
