using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// All supported voltage amplifiers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoltageAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    LABSMP21200 = 0,

    /// <summary>
    /// 
    /// </summary>
    SVG1200x00 = 1,

    /// <summary>
    /// 
    /// </summary>
    V210 = 2,

    /// <summary>
    /// 
    /// </summary>
    VU211x0 = 3,

    /// <summary>
    /// 
    /// </summary>
    VU211x1 = 4,

    /// <summary>
    /// 
    /// </summary>
    VU211x2 = 5,

    /// <summary>
    /// 
    /// </summary>
    VU220 = 6,

    /// <summary>
    /// 
    /// </summary>
    VU220x01 = 7,

    /// <summary>
    /// 
    /// </summary>
    VU220x02 = 8,

    /// <summary>
    /// 
    /// </summary>
    VU220x03 = 9,

    /// <summary>
    /// 
    /// </summary>
    VU220x04 = 10,

    /// <summary>
    /// 
    /// </summary>
    VU221x0 = 11,

    /// <summary>
    /// 
    /// </summary>
    VU221x0x2 = 12,

    /// <summary>
    /// 
    /// </summary>
    VU221x0x3 = 13,

    /// <summary>
    /// 
    /// </summary>
    VU221x1 = 14,

    /// <summary>
    /// 
    /// </summary>
    VU221x2 = 15,

    /// <summary>
    /// 
    /// </summary>
    VU221x3 = 16,

    /// <summary>
    /// 
    /// </summary>
    VUI301 = 17,

    /// <summary>
    /// 
    /// </summary>
    VUI302 = 18,

    /// <summary>
    /// 
    /// </summary>
    SVG3020 = 19,
}
