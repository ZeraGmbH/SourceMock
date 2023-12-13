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
    LABSMP715,

    /// <summary>
    /// 
    /// </summary>
    SCG1000x00,

    /// <summary>
    /// 
    /// </summary>
    SCG750x00,

    /// <summary>
    /// 
    /// </summary>
    V200,

    /// <summary>
    /// 
    /// </summary>
    V200x1x2,

    /// <summary>
    /// 
    /// </summary>
    V200x2,

    /// <summary>
    /// 
    /// </summary>
    V200x4,

    /// <summary>
    /// 
    /// </summary>
    VI201x0,

    /// <summary>
    /// 
    /// </summary>
    VI201x0x1,

    /// <summary>
    /// 
    /// </summary>
    VI201x1,

    /// <summary>
    /// 
    /// </summary>
    VI202x0,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x1,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x2,

    /// <summary>
    /// 
    /// </summary>
    VI202x0x5,

    /// <summary>
    /// 
    /// </summary>
    VI220,

    /// <summary>
    /// 
    /// </summary>
    VI221x0,

    /// <summary>
    /// 
    /// </summary>
    VI222x0,

    /// <summary>
    /// 
    /// </summary>
    VI222x0x1,

    /// <summary>
    /// 
    /// </summary>
    VUI301,

    /// <summary>
    /// 
    /// </summary>
    VUI302,

    /// <summary>
    /// 
    /// </summary>
    SCG1020,
}
