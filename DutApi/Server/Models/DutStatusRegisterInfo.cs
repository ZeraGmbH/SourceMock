using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutApi.Models;

/// <summary>
/// Data of a single register.
/// </summary>
public class DutStatusRegisterInfo
{
    /// <summary>
    /// The type of the register.
    /// </summary>
    [BsonElement("type"), Required, NotNull]
    public DutStatusRegisterTypes Type { get; set; }

    /// <summary>
    /// Address of the register, e.g. an SCPI name.
    /// </summary>
    [BsonElement("address")]
    public string? Address { get; set; }

    /// <summary>
    /// Optional scale for the value - based on the type
    /// this could be A, V oder Wh.
    /// </summary>
    [BsonElement("scale")]
    public double? Scale { get; set; }
}
