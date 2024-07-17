using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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
}