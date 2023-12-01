using System.Text.Json.Serialization;

namespace WebSamDeviceApis.Model;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CurrentAmplifiers
{
    /// <summary>
    /// 
    /// </summary>
    N132_7 = 29,
    /// <summary>
    /// 
    /// </summary>
    SCG1020 = 42,
    /// <summary>
    /// 
    /// </summary>
    V115_1 = 24,
    /// <summary>
    /// 
    /// </summary>
    V115_2 = 25,
    /// <summary>
    /// 
    /// </summary>
    V115_3 = 26,
    /// <summary>
    /// 
    /// </summary>
    V115_4 = 27,
    /// <summary>
    /// 
    /// </summary>
    V115_5 = 28,
    /// <summary>
    /// 
    /// </summary>
    V115_6 = 30,
    /// <summary>
    /// 
    /// </summary>
    V200_1 = 21,
    /// <summary>
    /// 
    /// </summary>
    V200_1_3 = 33,
    /// <summary>
    /// 
    /// </summary>
    V200_2 = 34,
    /// <summary>
    /// 
    /// </summary>
    V200_4 = 40,
    /// <summary>
    /// 
    /// </summary>
    VI201_0_1 = 39,
    /// <summary>
    /// 
    /// </summary>
    VI201_01 = 35,
    /// <summary>
    /// 
    /// </summary>
    VI202_0 = 36,
    /// <summary>
    /// 
    /// </summary>
    VI202_0_1 = 41,
    /// <summary>
    /// 
    /// </summary>
    VI202_0_2 = 43,
    /// <summary>
    /// 
    /// </summary>
    VI202_0_3 = 44,
    /// <summary>
    /// 
    /// </summary>
    VI202_0_4 = 45,
    /// <summary>
    /// 
    /// </summary>
    VI202_0_5 = 47,
    /// <summary>
    /// 
    /// </summary>
    VI220_1 = 22,
    /// <summary>
    /// 
    /// </summary>
    VI221_0 = 37,
    /// <summary>
    /// 
    /// </summary>
    VI222_0 = 38,
    /// <summary>
    /// 
    /// </summary>
    VI222_0_1 = 46,
    /// <summary>
    /// 
    /// </summary>
    VI301_1 = 23,
}
