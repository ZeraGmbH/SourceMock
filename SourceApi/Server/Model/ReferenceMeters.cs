using System.Text.Json.Serialization;

namespace WebSamDeviceApis.Model;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferenceMeters
{
    /// <summary>
    /// 
    /// </summary>
    COM3003 = 50,
    /// <summary>
    /// 
    /// </summary>
    COM3003_1_2 = 54,
    /// <summary>
    /// 
    /// </summary>
    COM3003_1_3 = 55,
    /// <summary>
    /// 
    /// </summary>
    COM3003DC = 51,
    /// <summary>
    /// 
    /// </summary>
    COM3003DC_1_2 = 61,
    /// <summary>
    /// 
    /// </summary>
    COM303_1 = 43,
    /// <summary>
    /// 
    /// </summary>
    COM303_2 = 45,
    /// <summary>
    /// 
    /// </summary>
    COM303_3 = 44,
    /// <summary>
    /// 
    /// </summary>
    COM303_3_1 = 57,
    /// <summary>
    /// 
    /// </summary>
    COM5003_1 = 65,
    /// <summary>
    /// 
    /// </summary>
    COM5003_1_1 = 71,
    /// <summary>
    /// 
    /// </summary>
    EPZ103_1 = 47,
    /// <summary>
    /// 
    /// </summary>
    EPZ103_1_2 = 60,
    /// <summary>
    /// 
    /// </summary>
    EPZ103_3_1 = 68,
    /// <summary>
    /// 
    /// </summary>
    EPZ301_119_3 = 52,
    /// <summary>
    /// 
    /// </summary>
    EPZ301_119_3_2 = 53,
    /// <summary>
    /// 
    /// </summary>
    EPZ303 = 46,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_1 = 41,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_10 = 70,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_10_1 = 72,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_5 = 42,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_5_1 = 56,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_5_2 = 58,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_6 = 59,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_8 = 62,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_8_1 = 64,
    /// <summary>
    /// 
    /// </summary>
    EPZ303_9 = 69,
    /// <summary>
    /// 
    /// </summary>
    EPZ350_0 = 66,
    /// <summary>
    /// 
    /// </summary>
    EPZ350_1 = 67,
    /// <summary>
    /// 
    /// </summary>
    RMM3000_1 = 63,
    /// <summary>
    /// 
    /// </summary>
    RMM303_6 = 48,
    /// <summary>
    /// 
    /// </summary>
    RMM303_8 = 49,
}
