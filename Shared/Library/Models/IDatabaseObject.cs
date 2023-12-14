namespace SharedLibrary.Models;

/// <summary>
/// Represents some database object with a required unique
/// identifier.
/// </summary>
public interface IDatabaseObject
{
    /// <summary>
    /// Unique identifer of the object which can be used
    /// as a primary key. Defaults to a new Guid.
    /// </summary>
    string Id { get; set; }
}

