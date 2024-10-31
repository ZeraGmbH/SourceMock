using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BarcodeApi.Models;
using BurdenApi.Models;
using ErrorCalculatorApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using SourceApi.Model.Configuration;
using ZERA.WebSam.Shared.Models;
using ZIFApi.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Connectivity configuration.
/// </summary>
public class InterfaceConfiguration
{
    /// <summary>
    /// Configuration of a serial port connection.
    /// </summary>
    [BsonElement("serial")]
    public SerialPortConfiguration? SerialPort { get; set; }

    /// <summary>
    /// Configuration for the barcode reader.
    /// </summary>
    [BsonElement("barcode")]
    public BarcodeConfiguration? Barcode { get; set; }

    /// <summary>
    /// Endpoint of the source REST web service.
    /// </summary>
    [BsonElement("source")]
    public RestConfiguration? Source { get; set; }

    /// <summary>
    /// Endpoint of the dosage REST web service - typically part of the source.
    /// </summary>
    [BsonElement("dosage")]
    public RestConfiguration? Dosage { get; set; }

    /// <summary>
    /// Endpoint of the reference meter REST web service.
    /// </summary>
    [BsonElement("refMeter")]
    public RestConfiguration? ReferenceMeter { get; set; }

    /// <summary>
    /// Endpoint to the meter test system itsself - e.g. to provide
    /// the firmware version.
    /// </summary>
    [BsonElement("meterTestSystem")]
    public RestConfiguration? MeterTestSystem { get; set; }

    /// <summary>
    /// List of error calculators to use.
    /// </summary>
    [BsonElement("errorCalculators")]
    public List<ErrorCalculatorConfiguration> ErrorCalculators { get; set; } = [];

    /// <summary>
    /// Configuration of optional ZIF sockets per test position.
    /// </summary>
    [BsonElement("zif")]
    public List<ZIFConfiguration> ZIFSockets { get; set; } = [];

    /// <summary>
    /// Configuration of custom serial port connections.
    /// </summary>
    [BsonElement("customSerialPorts")]
    public Dictionary<string, CustomSerialPortConfiguration> CustomSerialPorts { get; set; } = [];

    /// <summary>
    /// Burden to connect to - either voltage or current.
    /// </summary>
    [BsonElement("burden")]
    [NotNull, Required]
    public BurdenConfiguration Burden { get; set; } = new();
}
