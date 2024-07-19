using MeterTestSystemApi.Models.ConfigurationProviders;
using MongoDB.Bson.Serialization.Attributes;
using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Describes a probing operation which will be
/// stored in the database.
/// </summary>
public class ProbingOperation : IDatabaseObject
{
    /// <summary>
    /// Unique identifier of the operation.
    /// </summary>
    [BsonId]
    public required string Id { get; set; }

    /// <summary>
    /// Time the probing started.
    /// </summary>
    public required DateTime Created { get; set; }

    /// <summary>
    /// Time probing finished.
    /// </summary>
    public DateTime? Finished { get; set; }

    /// <summary>
    /// Configuration of the probing.
    /// </summary>
    public required ProbeConfigurationRequest Request { get; set; }

    /// <summary>
    /// Resulting configuration detected.
    /// </summary>
    public required ProbeConfigurationResult Result { get; set; }
}