using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrentAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    LABSMP715 = 0,

    /// <summary>
    /// 
    /// </summary>
    SCG1000x00 = 1,

    /// <summary>
    /// 
    /// </summary>
    SCG750x00 = 2,

    /// <summary>
    /// 
    /// </summary>
    V200 = 3,

    /// <summary>
    /// 
    /// </summary>
    V200x1x2 = 4,

    /// <summary>
    /// 
    /// </summary>
    V200x2 = 5,

    /// <summary>
    /// 
    /// </summary>
    V200x4 = 6,

    /// <summary>
    /// 
    /// </summary>
    VI201x0 = 7,

    /// <summary>
    /// 
    /// </summary>
    VI201x0x1 = 8,

    /// <summary>
    /// 
    /// </summary>
    VI201x1 = 9,

    /// <summary>
    /// 
    /// </summary>
    VI202x0 = 10,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x1 = 11,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x2 = 12,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x5 = 13,

    /// <summary>
    /// 
    /// </summary>
    VI220 = 14,

    /// <summary>
    /// 
    /// </summary>
    VI221x0 = 15,

    /// <summary>
    /// 
    /// </summary>
    VI222x0 = 16,

    /// <summary>
    /// 
    /// </summary>
    VI222x0x1 = 17,

    /// <summary>
    /// 
    /// </summary>
    VUI301 = 18,

    /// <summary>
    /// 
    /// </summary>
    VUI302 = 19,

    /// <summary>
    /// 
    /// </summary>
    SCG1020 = 20,
}
