using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SerialPortProxy;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Describe a serial port connection.
/// </summary>
public class SerialPortComponentConfiguration
{
    /// <summary>
    /// Index of the port as found in the device path.
    /// </summary>
    [NotNull, Required]
    public uint Index { get; set; }

    /// <summary>
    /// Type of the connection.
    /// </summary>
    [NotNull, Required]
    public SerialPortTypes Type { get; set; }

    /// <summary>
    /// Optional fine tuning of the configuration.
    /// </summary>
    public SerialPortOptions? Options { get; set; }
}