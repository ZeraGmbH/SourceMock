using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Services;

/// <summary>
/// All protocols available for a TCP/IP connection.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IPProbeProtocols
{
    /// <summary>
    /// Control of MP2020 via CR2020.
    /// </summary>
    MP2020Control = 0,

    /// <summary>
    /// STM6000 COM server.
    /// </summary>
    COMServer = 1,

    /// <summary>
    /// STM6000/STM4000 direct connection to device under test.
    /// </summary>
    COMServerDUT = 2,

    /// <summary>
    /// STM6000 object access.
    /// </summary>
    COMServerObjectAccess = 3,

    /// <summary>
    /// STM6000/STM4000 MAD server API version 1.
    /// </summary>
    MADServer1 = 4,

    /// <summary>
    /// STM6000/STM4000 MAD server API version 2.
    /// </summary>
    MADServer2 = 5,

    /// <summary>
    /// STM6000 SIM server 1.
    /// </summary>
    SIMServer1 = 6,

    /// <summary>
    /// STM6000/STM4000 COM Server UART.
    /// </summary>
    COMServerUART = 7,

    /// <summary>
    /// STM6000/STM4000 update server.
    /// </summary>
    UpdateServer = 8,

    /// <summary>
    /// STM6000 backend gateway.
    /// </summary>
    BackendGateway = 9,

    /// <summary>
    /// DC current amplifier.
    /// </summary>
    DCCurrent = 10,

    /// <summary>
    /// DC voltage amplifier.
    /// </summary>
    DCVoltage = 11,

    /// <summary>
    /// DC test system control.
    /// </summary>
    DCSPS = 12,

    /// <summary>
    /// DC test system with FG middleware in source.
    /// </summary>
    DCFGControl = 13,

    /// <summary>
    /// Transformer test system control.
    /// </summary>
    TransformerSPS = 14,

    /// <summary>
    /// Transformer STR 260 control.
    /// </summary>
    TransformerSTR260 = 15,

    /// <summary>
    /// Transformer current measurement.
    /// </summary>
    TransformerCurrent = 16,

    /// <summary>
    /// Transformer voltage measurement.
    /// </summary>
    TransformerVoltage = 17,

    /// <summary>
    /// Omega iBTHX temerature and humidity measurement.
    /// </summary>
    OmegaiBTHX = 18,

    /// <summary>
    /// External reference (absolute).
    /// </summary>
    COM5003 = 19,

    /// <summary>
    /// IP watchdog.
    /// </summary>
    IPWatchdog = 20,

    /// <summary>
    /// DTS100 keyboard.
    /// </summary>
    DTS100 = 21,

    /// <summary>
    /// NBox PLC router.
    /// </summary>
    NBoxRouter = 22,

    /// <summary>
    /// MTS310s2 reference meter EMob.
    /// </summary>
    MTS310s2EMob = 23,

    /// <summary>
    /// MTS310s2 reference meter DC source.
    /// </summary>
    MTS310s2DCSource = 24,

    /// <summary>
    /// MTS310s2 DC calibration for 4 position DC test system.
    /// </summary>
    MTS310s2Calibration = 25,
}