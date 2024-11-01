namespace MeterTestSystemApi.Services;

/// <summary>
/// All protocols available for a serial port connection.
/// </summary>
internal enum SerialProbeProtocols
{
    /// <summary>
    /// FG30x frequency generator.
    /// </summary>
    FG30x,

    /// <summary>
    /// MT768 compatible.
    /// </summary>
    MT768,

    /// <summary>
    /// Power Master Model 8121, ZIF socket.
    /// </summary>
    PM8181,

    /// <summary>
    /// ESVB/ESCB burden.
    /// </summary>
    ESxB,
}