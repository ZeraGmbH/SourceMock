using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// General configuration of the meter test system connected to the 
/// WebSam Server.
/// </summary>
public class MeterTestSystemConfiguration
{
    /// <summary>
    /// Type of the connected meter test system - use to decide
    /// what communication protocols to use.
    /// </summary>
    [BsonIgnore]
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
}