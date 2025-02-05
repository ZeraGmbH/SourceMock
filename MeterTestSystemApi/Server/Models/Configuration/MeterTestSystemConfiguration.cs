using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ZERA.WebSam.Shared.Models.MeterTestSystem;
using MongoDB.Bson.Serialization.Attributes;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// General configuration of the meter test system connected to the 
/// WebSam Server.
/// </summary>
public class MeterTestSystemConfiguration
{
    /// <summary>
    /// Set if WebSAM is started in probing mode.
    /// </summary>
    [BsonElement("probing")]
    [NotNull, Required]
    public bool Probing { get; set; } = false;

    /// <summary>
    /// Type of the connected meter test system - use to decide
    /// what communication protocols to use.
    /// </summary>
    [BsonElement("type")]
    public MeterTestSystemTypes? MeterTestSystemType { get; set; }

    /// <summary>
    /// Amplifier configuration if applicable.
    /// </summary>
    [BsonElement("amplifiers")]
    public AmplifiersAndReferenceMeter? AmplifiersAndReferenceMeter { get; set; }

    /// <summary>
    /// Connectivity configuration.
    /// </summary>
    [NotNull, Required]
    [BsonElement("interfaces")]
    public InterfaceConfiguration Interfaces { get; set; } = new();

    /// <summary>
    /// Set to do not use the source if possible. Not all meter test
    /// systems will allow this.
    /// </summary>
    [BsonElement("noSource")]
    public bool? NoSource { get; set; }

    /// <summary>
    /// Optional external reference meter to use.
    /// </summary>
    [BsonElement("refMeter")]
    public ExternalReferenceMeterConfiguration? ExternalReferenceMeter { get; set; }

    /// <summary>
    /// Optional SMTP Mail server configuration.
    /// </summary>
    [BsonElement("smtp")]
    public SmtpConfiguration? Smtp { get; set; }
}