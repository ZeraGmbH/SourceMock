using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// All supported voltage amplifiers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoltageAuxiliaries
{
    /// <summary>
    /// 
    /// </summary>
    SVG150x00 = 0,

    /// <summary>
    /// 
    /// </summary>
    V210 = 1,

    /// <summary>
    /// 
    /// </summary>
    VU211x0 = 2,

    /// <summary>
    /// 
    /// </summary>
    VU211x1 = 3,

    /// <summary>
    /// 
    /// </summary>
    VU211x2 = 4,

    /// <summary>
    /// 
    /// </summary>
    VU220 = 5,

    /// <summary>
    /// 
    /// </summary>
    VU220x01 = 6,

    /// <summary>
    /// 
    /// </summary>
    VU220x02 = 7,

    /// <summary>
    /// 
    /// </summary>
    VU220x03 = 8,

    /// <summary>
    /// 
    /// </summary>
    VU220x04 = 9,

    /// <summary>
    /// 
    /// </summary>
    VU220x6 = 10,

    /// <summary>
    /// 
    /// </summary>
    VU221x0 = 11,

    /// <summary>
    /// 
    /// </summary>
    VU221x1 = 12,

    /// <summary>
    /// 
    /// </summary>
    VU221x2 = 13,

    /// <summary>
    /// 
    /// </summary>
    VU221x3 = 14,

    /// <summary>
    /// 
    /// </summary>
    VUI301 = 15,

    /// <summary>
    /// 
    /// </summary>
    VUI302 = 16,
}
