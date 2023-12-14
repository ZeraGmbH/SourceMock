namespace SharedLibrary.Models;

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
public interface IHistoryCollection<T> : IObjectCollection<T> where T : IDatabaseObject
{
    /// <summary>
    /// Find items and add the full history information.
    /// </summary>
    /// <param name="id">Primary key of the document the history should be reported.</param>
    /// <returns>History of the document sorted by version descending - i.e. newest first.</returns>
    Task<IEnumerable<HistoryItem<T>>> GetHistory(string id);
}
