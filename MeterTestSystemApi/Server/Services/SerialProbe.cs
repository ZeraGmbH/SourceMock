using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Services;

/// <summary>
/// Describe a single serial port probing.
/// </summary>
public class SerialProbe : Probe
{
    /// <summary>
    /// Protocol to use.
    /// </summary>
    [NotNull, Required]
    public SerialProbeProtocols Protocol { get; set; }

    /// <summary>
    /// Device to use.
    /// </summary>
    [NotNull, Required]
    public SerialPortComponentConfiguration Device { get; set; } = null!;

    /// <summary>
    /// Create a description for the probe.
    /// </summary>
    public override string ToString() => $"{DevicePath}: {Protocol}";

    /// <summary>
    /// Get the device path of this connection.
    /// </summary>
    [JsonIgnore]
    public string DevicePath => $"/dev/tty{Device.Type switch
    {
        SerialPortTypes.RS232 => "S",
        SerialPortTypes.USB => "USB",
        _ => throw new ArgumentException($"unknown serial port type {Device.Type}")
    }}{Device.Index}";

}

