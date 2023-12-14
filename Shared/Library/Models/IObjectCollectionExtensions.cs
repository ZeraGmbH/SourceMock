namespace SharedLibrary.Models;

/// <summary>
/// Some helper methods for collections.
/// </summary>
public static class IObjectCollectionExtensions
{
    /// <summary>
    /// Find a single document.
    /// </summary>
    /// <typeparam name="TItem">Type of the document.</typeparam>
    /// <param name="collection">Collection implementation to expand.</param>
    /// <param name="id">Unique identifier of the document.</param>
    /// <returns>Task resolving to the document.</returns>
    public static Task<TItem?> GetItem<TItem>(this IObjectCollection<TItem> collection, string id) where TItem : IDatabaseObject =>
        Task.Run(() => collection.CreateQueryable().Where(i => i.Id == id).SingleOrDefault());
}
