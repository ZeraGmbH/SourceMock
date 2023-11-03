using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DeviceApiSharedLibrary.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace RefMeterApi.Models;

/// <summary>
/// Describes the data for a new device under test.
/// </summary>
public class NewDeviceUnderTest
{
    /// <summary>
    /// The name of the device.
    /// </summary>
    [Required]
    [MinLength(1)]
    [NotNull]
    public string Name { get; set; } = null!;
}


/// <summary>
/// Describes a device under test.
/// </summary>
[BsonIgnoreExtraElements]
public class DeviceUnderTest : DatabaseObject
{
    /// <summary>
    /// The name of the device.
    /// </summary>
    [BsonElement("name")]
    [Required]
    [NotNull]
    public string Name { get; set; } = null!;
}
