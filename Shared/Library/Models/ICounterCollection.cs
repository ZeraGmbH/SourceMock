namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface ICounterCollection
{
    /// <summary>
    /// Get the next value for a counter - start with 0 if counter
    /// does not yet exist.
    /// </summary>
    /// <param name="name">Name of the counter.</param>
    /// <returns>Next value of the counter.</returns>
    Task<long> GetNextCounter(string name);
}
