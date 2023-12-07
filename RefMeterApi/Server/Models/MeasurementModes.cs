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
    FourWireActivePower,

    /// <summary>
    /// 4WR, 4LBE
    /// </summary>
    FourWireReactivePower,

    /// <summary>
    /// 4WRC, 4LBK
    /// </summary>
    FourWireReactivePowerCrossConnected,

    /// <summary>
    /// 4WAP, 4LS
    /// </summary>
    FourWireApparentPower,

    /// <summary>
    /// 3WA, 3LW
    /// </summary>
    ThreeWireActivePower,

    /// <summary>
    /// 3WR, 3LBE 
    /// </summary>
    ThreeWireReactivePower,

    /// <summary>
    /// 3WRCA, 3LBKA
    /// </summary>
    ThreeWireReactivePowerCrossConnectedA,

    /// <summary>
    /// 3WRCB, 3LBKB
    /// </summary>
    ThreeWireReactivePowerCrossConnectedB,

    /// <summary>
    /// 3WAP, 3LS
    /// </summary>
    ThreeWireApparentPower,

    /// <summary>
    /// 2WA, 2LW
    /// </summary>
    TwoWireActivePower,

    /// <summary>
    /// 2WR, 2LB
    /// </summary>
    TwoWireReactivePower,

    /// <summary>
    /// 2WAP, 2LS
    /// </summary>
    TwoWireApparentPower,

    /// <summary>
    /// 1PHA
    /// </summary>
    PhaseTApparentPower,

    /// <summary>
    /// 1PHR
    /// </summary>
    PhaseTReactivePower,

    /// <summary>
    /// 1PHT
    /// </summary>
    PhaseTPower,

    /// <summary>
    /// 3LBG
    /// </summary>
    ThreeWireReactiveGeometricPower,

    /// <summary>
    /// 3LQ6
    /// </summary>
    ThreeWireReactive60Power,

    /// <summary>
    /// 3LSG
    /// </summary>
    ThreeWireApparentGeometricPower,

    /// <summary>
    /// 3LWR
    /// </summary>
    ThreeWireSymmetricPower,

    /// <summary>
    /// 3Q6K
    /// </summary>
    ThreeWireReactive60SyntheticPower,

    /// <summary>
    /// 4LBF
    /// </summary>
    FourWireReactiveRtsPower,

    /// <summary>
    /// 4LQ6
    /// </summary>
    FourWireReactive60Power,

    /// <summary>
    /// 4LSG
    /// </summary>
    FourWireApparentGeometricPower,

    /// <summary>
    /// 4Q6K
    /// </summary>
    FourWireReactive60SyntheticPower,

    /// <summary>
    /// 4LDC
    /// </summary>
    FourWireDCPower,
}
