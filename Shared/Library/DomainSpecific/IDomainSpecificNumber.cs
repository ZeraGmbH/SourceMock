namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public interface IDomainSpecificNumber
{
    /// <summary>
    /// The natural unit of the number.
    /// </summary>
    string Unit { get; }
}
