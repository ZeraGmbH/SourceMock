using System.Text.Json.Serialization;

namespace SourceApi.Model;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoltageAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    N132x3 = 13,
    /// <summary>
    /// 
    /// </summary>
    N132x4 = 14,
    /// <summary>
    /// 
    /// </summary>
    N132x5 = 15,
    /// <summary>
    /// 
    /// </summary>
    N132x6 = 21,
    /// <summary>
    /// 
    /// </summary>
    N132x9 = 16,
    /// <summary>
    /// 
    /// </summary>
    SVG3020x0 = 37,
    /// <summary>
    /// 
    /// </summary>
    V114x2 = 4,
    /// <summary>
    /// 
    /// </summary>
    V114x3 = 5,
    /// <summary>
    /// 
    /// </summary>
    V114x4 = 6,
    /// <summary>
    /// 
    /// </summary>
    V114x5 = 7,
    /// <summary>
    /// 
    /// </summary>
    V114x5x2 = 24,
    /// <summary>
    /// 
    /// </summary>
    V114x6 = 23,
    /// <summary>
    /// 
    /// </summary>
    V116x1 = 8,
    /// <summary>
    /// 
    /// </summary>
    V116x2 = 9,
    /// <summary>
    /// 
    /// </summary>
    V116x2x2 = 22,
    /// <summary>
    /// 
    /// </summary>
    V116x4 = 10,
    /// <summary>
    /// 
    /// </summary>
    V116x5 = 11,
    /// <summary>
    /// 
    /// </summary>
    V116x7 = 12,
    /// <summary>
    /// 
    /// </summary>
    V210x1 = 1,
    /// <summary>
    /// 
    /// </summary>
    V210x1x2 = 28,
    /// <summary>
    /// 
    /// </summary>
    V210x1x3 = 29,
    /// <summary>
    /// 
    /// </summary>
    VU211x012 = 30,
    /// <summary>
    /// 
    /// </summary>
    VU220 = 2,
    /// <summary>
    /// 
    /// </summary>
    VU220x1 = 18,
    /// <summary>
    /// 
    /// </summary>
    VU220x1x1 = 27,
    /// <summary>
    /// 
    /// </summary>
    VU220x2 = 17,
    /// <summary>
    /// 
    /// </summary>
    VU220x2x2 = 25,
    /// <summary>
    /// 
    /// </summary>
    VU220x3 = 19,
    /// <summary>
    /// 
    /// </summary>
    VU220x4 = 20,
    /// <summary>
    /// 
    /// </summary>
    VU220x4x1 = 26,
    /// <summary>
    /// 
    /// </summary>
    VU220x4x2 = 34,
    /// <summary>
    /// 
    /// </summary>
    VU221x0 = 31,
    /// <summary>
    /// 
    /// </summary>
    VU221x0x1 = 35,
    /// <summary>
    /// 
    /// </summary>
    VU221x0x2 = 36,
    /// <summary>
    /// 
    /// </summary>
    VU221x0x3 = 38,
    /// <summary>
    /// 
    /// </summary>
    VU221x13 = 32,
    /// <summary>
    /// 
    /// </summary>
    VU221x2 = 33,
    /// <summary>
    /// 
    /// </summary>
    VU301x1 = 3,
}
