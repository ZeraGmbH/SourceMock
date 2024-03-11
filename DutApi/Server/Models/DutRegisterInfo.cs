using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DutApi.Models;

/// <summary>
/// Data of a single register.
/// </summary>
public class DutRegisterInfo
{
    /// <summary>
    /// The type of the register.
    /// </summary>
    [BsonElement("type"), Required, NotNull]
    public DutRegisterTypes Type { get; set; }

    /// <summary>
    /// Address of the register, e.g. an OBIS code.
    /// </summary>
    [BsonElement("address"), Required, NotNull, MinLength(1)]
    public string Address { get; set; } = null!;

    /// <summary>
    /// Unit of the value.
    /// </summary>
    [BsonElement("unit"), Required, NotNull, DefaultValue(DutRegisterUnits.Wh)]
    public DutRegisterUnits Unit { get; set; }

    /// <summary>
    /// In the current context register values are all numbers
    /// with kWh as a unit. The scale will define how to get
    /// the value in Wh: the raw register value will be multiplied
    /// with the scale to get the protocol value.
    /// </summary>
    [BsonElement("scale"), Required, NotNull]
    public double Scale { get; set; }
}
