using System.Linq.Expressions;

namespace SharedLibrary.Models;

/// <summary>
/// 
/// </summary>
public interface ITestableObjectCollection
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
public interface IBasicObjectCollection<T> : ITestableObjectCollection where T : IDatabaseObject
{
    /// <summary>
    /// Add a brand new document to the collection.
    /// </summary>
    /// <param name="item">The data for the new item.</param>
    /// <returns>The item as added to the database.</returns>
    Task<T> AddItem(T item);

    /// <summary>
    /// Update an existing item.
    /// </summary>
    /// <param name="item">New item data.</param>
    /// <returns>The new item data.</returns>
    /// <exception cref="ArgumentException">There is no such item.</exception>
    Task<T> UpdateItem(T item);

    /// <summary>
    /// Delete an existing item.
    /// </summary>
    /// <param name="id">The unique identifier of the item.</param>
    /// <param name="silent">Set to avoid an exception if the item does not exist.</param>
    /// <returns>The item which has been deleted.</returns>
    /// <exception cref="ArgumentException">There is no such item.</exception>
    Task<T> DeleteItem(string id, bool silent = false);

    /// <summary>
    /// Start a query on the collection.
    /// </summary>
    /// <param name="batchSize">Number of items per batch operation.</param>
    /// <returns>A new queryable.</returns>
    IQueryable<T> CreateQueryable(int? batchSize = null);

    /// <summary>
    /// Create a new index.
    /// </summary>
    /// <param name="ascending">Unset to use an descending index.</param>
    /// <param name="caseSensitive">Unset to use a case insensitive index.</param>
    /// <param name="keyAccessor">Accessor to the field to use.</param>
    /// <param name="name">Name the the index.</param>
    /// <param name="unique">Unset to use an index without a unique constraint.</param>
    /// <returns>Task waiting for the index to be created.</returns>
    Task<string> CreateIndex(string name, Expression<Func<T, object>> keyAccessor, bool ascending = true, bool unique = true, bool caseSensitive = true);
}

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
public interface IObjectCollection<T> : IBasicObjectCollection<T> where T : IDatabaseObject
{
    /// <summary>
    /// Delete a list of items preferrable with a single database operation.
    /// </summary>
    /// <param name="filter">Filter condition to detect the documents to delete.</param>
    /// <returns>Number of deleted documents.</returns>
    Task<long> DeleteItems(Expression<Func<T, bool>> filter);
}

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
/// <typeparam name="TCommon"></typeparam>
public interface IBasicObjectCollection<T, TCommon> : IBasicObjectCollection<T> where T : IDatabaseObject where TCommon : ICollectionInitializer<T>
{
    /// <summary>
    /// 
    /// </summary>
    TCommon Common { get; }
}

/// <summary>
/// Base class for implementing historized collections.
/// </summary>
/// <typeparam name="T">Type of the item to use</typeparam>
/// <typeparam name="TCommon"></typeparam>
public interface IObjectCollection<T, TCommon> : IBasicObjectCollection<T, TCommon>, IObjectCollection<T> where T : IDatabaseObject where TCommon : ICollectionInitializer<T>
{
}
