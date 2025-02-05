using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SerialPortProxy;

namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
public class ExternalReferenceMeterConfiguration
{
    /// <summary>
    /// Type of the external reference meter.
    /// </summary>
    [NotNull, Required]
    public ExternalReferenceMeterTypes Type { get; set; }

    /// <summary>
    /// Serial port to connect to the reference meter.
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}