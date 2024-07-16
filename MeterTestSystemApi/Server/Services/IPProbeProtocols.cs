namespace MeterTestSystemApi.Services;

/// <summary>
/// All protocols available for a TCP/IP connection.
/// </summary>
internal enum IPProbeProtocols
{
    /// <summary>
    /// Control of MP2020 via CR2020.
    /// </summary>
    MP2020Control,

    /// <summary>
    /// STM6000 COM server.
    /// </summary>
    COMServer,

    /// <summary>
    /// STM6000/STM4000 direct connection to device under test.
    /// </summary>
    COMServerDUT,

    /// <summary>
    /// STM6000 object access.
    /// </summary>
    COMServerObjectAccess,

    /// <summary>
    /// STM6000/STM4000 MAD server API version 1.
    /// </summary>
    MADServer1,

    /// <summary>
    /// STM6000/STM4000 MAD server API version 2.
    /// </summary>
    MADServer2,

    /// <summary>
    /// STM6000 SIM server 1.
    /// </summary>
    SIMServer1,

    /// <summary>
    /// STM6000/STM4000 COM Server UART.
    /// </summary>
    COMServerUART,

    /// <summary>
    /// STM6000/STM4000 update server.
    /// </summary>
    UpdateServer,

    /// <summary>
    /// STM6000 backend gateway.
    /// </summary>
    BackendGateway,

    /// <summary>
    /// DC current amplifier.
    /// </summary>
    DCCurrent,

    /// <summary>
    /// DC voltage amplifier.
    /// </summary>
    DCVoltage,

    /// <summary>
    /// DC test system control.
    /// </summary>
    DCSPS,

    /// <summary>
    /// DC test system with FG middleware in source.
    /// </summary>
    DCFGControl,

    /// <summary>
    /// Transformer test system control.
    /// </summary>
    TransformerSPS,

    /// <summary>
    /// Transformer STR 260 control.
    /// </summary>
    TransformerSTR260,

    /// <summary>
    /// Transformer current measurement.
    /// </summary>
    TransformerCurrent,

    /// <summary>
    /// Transformer voltage measurement.
    /// </summary>
    TransformerVoltage,

    /// <summary>
    /// Omega iBTHX temerature and humidity measurement.
    /// </summary>
    OmegaiBTHX,

    /// <summary>
    /// External reference (absolute).
    /// </summary>
    COM5003,

    /// <summary>
    /// IP watchdog.
    /// </summary>
    IPWatchdog,

    /// <summary>
    /// DTS100 keyboard.
    /// </summary>
    DTS100,

    /// <summary>
    /// NBox PLC router.
    /// </summary>
    NBoxRouter,

    /// <summary>
    /// MTS310s2 reference meter EMob.
    /// </summary>
    MTS310s2EMob,

    /// <summary>
    /// MTS310s2 reference meter DC source positions 1 to 4.
    /// </summary>
    MTS310s2DC1,

    /// <summary>
    /// MTS310s2 reference meter DC source positions 5 to 8.
    /// </summary>
    MTS310s2DC2,

    /// <summary>
    /// MTS310s2 DC calibration for 4 position DC test system.
    /// </summary>
    MTS310s2Calibration,
}