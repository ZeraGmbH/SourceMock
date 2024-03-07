using ErrorCalculatorApi.Models;
using MongoDB.Bson.Serialization.Attributes;
using SharedLibrary.Models;
using SourceApi.Model.Configuration;

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
}
