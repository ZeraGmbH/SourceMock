using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// All supported measurement modes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MeasurementModes
{
    /// <summary>
    /// 4WA, 4LW
    /// </summary>
    FourWireActivePower = 0,

    /// <summary>
    /// 4WR, 4LBE
    /// </summary>
    FourWireReactivePower = 1,

    /// <summary>
    /// 4WRC, 4LBK
    /// </summary>
    FourWireReactivePowerCrossConnected = 2,

    /// <summary>
    /// 4WAP, 4LS
    /// </summary>
    FourWireApparentPower = 3,

    /// <summary>
    /// 3WA, 3LW
    /// </summary>
    ThreeWireActivePower = 4,

    /// <summary>
    /// 3WR, 3LBE 
    /// </summary>
    ThreeWireReactivePower = 5,

    /// <summary>
    /// 3WRCA, 3LBKA
    /// </summary>
    ThreeWireReactivePowerCrossConnectedA = 6,

    /// <summary>
    /// 3WRCB, 3LBKB
    /// </summary>
    ThreeWireReactivePowerCrossConnectedB = 7,

    /// <summary>
    /// 3WAP, 3LS
    /// </summary>
    ThreeWireApparentPower = 8,

    /// <summary>
    /// 2WA, 2LW
    /// </summary>
    TwoWireActivePower = 9,

    /// <summary>
    /// 2WR, 2LB
    /// </summary>
    TwoWireReactivePower = 10,

    /// <summary>
    /// 2WAP, 2LS
    /// </summary>
    TwoWireApparentPower = 11,

    /// <summary>
    /// MQRef
    /// </summary>
    MqRef = 12,

    /// <summary>
    /// MQBase
    /// </summary>
    MqBase = 13,
}
