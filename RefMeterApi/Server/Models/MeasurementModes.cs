using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// All supported measurement modes.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MeasurementModes
{
    /// <summary>
    /// 4WA
    /// </summary>
    FourWireActivePower,

    /// <summary>
    /// 4WR
    /// </summary>
    FourWireReactivePower,

    /// <summary>
    /// 4WRC
    /// </summary>
    FourWireReactivePowerCrossConnected,

    /// <summary>
    /// 4WAP
    /// </summary>
    FourWireApparentPower,

    /// <summary>
    /// 3WA
    /// </summary>
    ThreeWireActivePower,

    /// <summary>
    /// 3WR
    /// </summary>
    ThreeWireReactivePower,

    /// <summary>
    /// 3WRCA
    /// </summary>
    ThreeWireReactivePowerCrossConnectedA,

    /// <summary>
    /// 3WRCB
    /// </summary>
    ThreeWireReactivePowerCrossConnectedB,

    /// <summary>
    /// 3WAP
    /// </summary>
    ThreeWireApparentPower,

    /// <summary>
    /// 2WA
    /// </summary>
    TwoWireActivePower,

    /// <summary>
    /// 2WR
    /// </summary>
    TwoWireReactivePower,

    /// <summary>
    /// 2WAP
    /// </summary>
    TwoWireApparentPower,
}
