using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Resources;
using MeterTestSystemApi.Services;
using MongoDB.Driver;
using SerialPortProxy;
using SourceApi.Model.Configuration;

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

    /// <summary>
    /// Make it an actual configuration.
    /// </summary>
    /// <returns>Runtime configuration.</returns>
    public SerialPortConfiguration ToLive(SerialProbeProtocols protocol)
    {
        var probe = new SerialProbe { Protocol = protocol, Device = this };

        return new()
        {
            Authorization = null,
            ConfigurationType = SerialPortConfigurationTypes.Device,
            Endpoint = probe.DevicePath,
            SerialPortOptions = Options,
        };
    }
}