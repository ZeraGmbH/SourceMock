namespace DeviceApiSharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface IObjectCollection
{
    /// <summary>
    /// Remove all documents related with this collection.
    /// </summary>
    /// <returns>Number of documents deleted.</returns>
    Task<long> RemoveAll();
}

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
public interface IObjectCollection<T> : IObjectCollection where T : IDatabaseObject
{
    /// <summary>
    /// Add a brand new document to the collection.
    /// </summary>
    /// <param name="item">The data for the new item.</param>
    /// <param name="user">Name of the user adding the document.</param>
    /// <returns>The item as added to the database.</returns>
    Task<T> AddItem(T item, string user);

    /// <summary>
    /// Update an existing item.
    /// </summary>
    /// <param name="item">New item data.</param>
    /// <param name="user">User requesting the update.</param>
    /// <returns>The new item data.</returns>
    /// <exception cref="ArgumentException">There is no such item.</exception>
    Task<T> UpdateItem(T item, string user);

    /// <summary>
    /// Delete an existing item.
    /// </summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="user">The user deleting the item.</param>
    /// <returns>The item which has been deleted.</returns>
    /// <exception cref="ArgumentException">There is no such item.</exception>
    Task<T> DeleteItem(string id, string user);

    /// <summary>
    /// Start a query on the collection.
    /// </summary>
    /// <returns>A new queryable.</returns>
    IQueryable<T> CreateQueryable();
}
