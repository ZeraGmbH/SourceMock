using System.Text.Json.Serialization;

namespace FrequencyGeneratorApi.Models;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrentAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    N132x7 = 29,
    /// <summary>
    /// 
    /// </summary>
    SCG1020 = 42,
    /// <summary>
    /// 
    /// </summary>
    V115x1 = 24,
    /// <summary>
    /// 
    /// </summary>
    V115x2 = 25,
    /// <summary>
    /// 
    /// </summary>
    V115x3 = 26,
    /// <summary>
    /// 
    /// </summary>
    V115x4 = 27,
    /// <summary>
    /// 
    /// </summary>
    V115x5 = 28,
    /// <summary>
    /// 
    /// </summary>
    V115x6 = 30,
    /// <summary>
    /// 
    /// </summary>
    V200x1 = 21,
    /// <summary>
    /// 
    /// </summary>
    V200x1x3 = 33,
    /// <summary>
    /// 
    /// </summary>
    V200x2 = 34,
    /// <summary>
    /// 
    /// </summary>
    V200x4 = 40,
    /// <summary>
    /// 
    /// </summary>
    VI201x0x1 = 39,
    /// <summary>
    /// 
    /// </summary>
    VI201x01 = 35,
    /// <summary>
    /// 
    /// </summary>
    VI202x0 = 36,
    /// <summary>
    /// 
    /// </summary>
    VI202x0x1 = 41,
    /// <summary>
    /// 
    /// </summary>
    VI202x0x2 = 43,
    /// <summary>
    /// 
    /// </summary>
    VI202x0x3 = 44,
    /// <summary>
    /// 
    /// </summary>
    VI202x0x4 = 45,
    /// <summary>
    /// 
    /// </summary>
    VI202x0x5 = 47,
    /// <summary>
    /// 
    /// </summary>
    VI220x1 = 22,
    /// <summary>
    /// 
    /// </summary>
    VI221x0 = 37,
    /// <summary>
    /// 
    /// </summary>
    VI222x0 = 38,
    /// <summary>
    /// 
    /// </summary>
    VI222x0x1 = 46,
    /// <summary>
    /// 
    /// </summary>
    VI301x1 = 23,
}
