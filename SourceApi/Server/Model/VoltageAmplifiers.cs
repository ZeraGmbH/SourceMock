using System.Text.Json.Serialization;

namespace WebSamDeviceApis.Model;

/// <summary>
/// Contains the information about a simulated source
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoltageAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    N132_3 = 13,
    /// <summary>
    /// 
    /// </summary>
    N132_4 = 14,
    /// <summary>
    /// 
    /// </summary>
    N132_5 = 15,
    /// <summary>
    /// 
    /// </summary>
    N132_6 = 21,
    /// <summary>
    /// 
    /// </summary>
    N132_9 = 16,
    /// <summary>
    /// 
    /// </summary>
    SVG3020_0 = 37,
    /// <summary>
    /// 
    /// </summary>
    V114_2 = 4,
    /// <summary>
    /// 
    /// </summary>
    V114_3 = 5,
    /// <summary>
    /// 
    /// </summary>
    V114_4 = 6,
    /// <summary>
    /// 
    /// </summary>
    V114_5 = 7,
    /// <summary>
    /// 
    /// </summary>
    V114_5_2 = 24,
    /// <summary>
    /// 
    /// </summary>
    V114_6 = 23,
    /// <summary>
    /// 
    /// </summary>
    V116_1 = 8,
    /// <summary>
    /// 
    /// </summary>
    V116_2 = 9,
    /// <summary>
    /// 
    /// </summary>
    V116_2_2 = 22,
    /// <summary>
    /// 
    /// </summary>
    V116_4 = 10,
    /// <summary>
    /// 
    /// </summary>
    V116_5 = 11,
    /// <summary>
    /// 
    /// </summary>
    V116_7 = 12,
    /// <summary>
    /// 
    /// </summary>
    V210_1 = 1,
    /// <summary>
    /// 
    /// </summary>
    V210_1_2 = 28,
    /// <summary>
    /// 
    /// </summary>
    V210_1_3 = 29,
    /// <summary>
    /// 
    /// </summary>
    VU211_012 = 30,
    /// <summary>
    /// 
    /// </summary>
    VU220 = 2,
    /// <summary>
    /// 
    /// </summary>
    VU220_1 = 18,
    /// <summary>
    /// 
    /// </summary>
    VU220_1_1 = 27,
    /// <summary>
    /// 
    /// </summary>
    VU220_2 = 17,
    /// <summary>
    /// 
    /// </summary>
    VU220_2_2 = 25,
    /// <summary>
    /// 
    /// </summary>
    VU220_3 = 19,
    /// <summary>
    /// 
    /// </summary>
    VU220_4 = 20,
    /// <summary>
    /// 
    /// </summary>
    VU220_4_1 = 26,
    /// <summary>
    /// 
    /// </summary>
    VU220_4_2 = 34,
    /// <summary>
    /// 
    /// </summary>
    VU221_0 = 31,
    /// <summary>
    /// 
    /// </summary>
    VU221_0_1 = 35,
    /// <summary>
    /// 
    /// </summary>
    VU221_0_2 = 36,
    /// <summary>
    /// 
    /// </summary>
    VU221_0_3 = 38,
    /// <summary>
    /// 
    /// </summary>
    VU221_13 = 32,
    /// <summary>
    /// 
    /// </summary>
    VU221_2 = 33,
    /// <summary>
    /// 
    /// </summary>
    VU301_1 = 3,
}
