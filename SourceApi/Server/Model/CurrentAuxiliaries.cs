using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrentAuxiliaries
{
    /// <summary>
    /// 
    /// </summary>
    V200 = 0,

    /// <summary>
    /// 
    /// </summary>
    VI220 = 1,

    /// <summary>
    /// 
    /// </summary>
    VUI301 = 2,

    /// <summary>
    /// 
    /// </summary>
    V200x2 = 3,

    /// <summary>
    /// 
    /// </summary>
    VI201x0 = 4,

    /// <summary>
    /// 
    /// </summary>
    VI201x1 = 5,

    /// <summary>
    /// 
    /// </summary>
    VI202x0 = 6,

    /// <summary>
    /// 
    /// </summary>
    VI221x0 = 7,

    /// <summary>
    /// 
    /// </summary>
    VI222x0 = 8,

    /// <summary>
    /// 
    /// </summary>
    VI201x0x1 = 9,

    /// <summary>
    /// 
    /// </summary>
    VI200x4 = 10,

    /// <summary>
    /// 
    /// </summary>
    VUI302 = 11,
}

