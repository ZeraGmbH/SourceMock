using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// Describe an interface activity.
/// </summary>
public class InterfaceLogEntry : IDatabaseObject
{
    /// <summary>
    /// Unique identifier of the database entry.
    /// </summary>
    [NotNull, Required]
    public required string Id { get; set; }

    /// <summary>
    /// Connection to the device.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryConnection Connection { get; set; }

    /// <summary>
    /// The context of the communication.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryScope Scope { get; set; }

    /// <summary>
    /// System information on the log entry.
    /// </summary>
    [NotNull, Required]
    public required InterfaceLogEntryInfo Info { get; set; }

    /// <summary>
    /// Payload of the data.
    /// </summary>
    public InterfaceLogPayload Message { get; set; } = null!;
}