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
    V200,

    /// <summary>
    /// 
    /// </summary>
    VI220,

    /// <summary>
    /// 
    /// </summary>
    VUI301,

    /// <summary>
    /// 
    /// </summary>
    V200x2,

    /// <summary>
    /// 
    /// </summary>
    VI201x0,

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
    VI221x0,

    /// <summary>
    /// 
    /// </summary>
    VI222x0,

    /// <summary>
    /// 
    /// </summary>
    VI201x0x1,

    /// <summary>
    /// 
    /// </summary>
    VI200x4,

    /// <summary>
    /// 
    /// </summary>
    VUI302,
}

