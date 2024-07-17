using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a single serial port probing.
/// </summary>
internal class SerialProbe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    public required SerialProbeProtocols Protocol { get; set; }

    /// <summary>
    /// Device to use.
    /// </summary>
    public required SerialPortConfiguration Device { get; set; }

    /// <summary>
    /// Set the result of the probing.
    /// </summary>
    public ProbeResult Result { get; set; }

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{DevicePath}: {Protocol}";

    /// <summary>
    /// Get the device path of this connection.
    /// </summary>
    public string DevicePath => $"/dev/tty{Device.Type switch
    {
        SerialPortTypes.RS232 => "S",
        SerialPortTypes.USB => "USB",
        _ => throw new ArgumentException($"unknown serial port type {Device.Type}")
    }}{Device.Index}";

}

