
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;

namespace DeviceApiSharedLibrary.Actions.Database;

/// <summary>
/// MongoDb collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
public abstract class MongoDbCollection<TItem> : IObjectCollection<TItem> where TItem : DatabaseObject
{
    /// <summary>
    /// Name of the collection to use.
    /// </summary>
    public abstract string CollectionName { get; }

    private readonly IMongoDbDatabaseService _database;

    private readonly ILogger<MongoDbCollection<TItem>> _logger;

    /// <summary>
    /// Initializes a new collection.
    /// </summary>
    /// <param name="database">Database connection to use.</param>
    /// <param name="logger">Logging instance to use.</param>
    public MongoDbCollection(IMongoDbDatabaseService database, ILogger<MongoDbCollection<TItem>> logger)
    {
        _database = database;
        _logger = logger;
    }

    private IMongoCollection<T> GetCollection<T>() => _database.Database.GetCollection<T>(CollectionName);

    private IMongoCollection<TItem> GetCollection() => GetCollection<TItem>();

    /// <inheritdoc/>
    public virtual Task<TItem> AddItem(TItem item, string user) => GetCollection()
        .InsertOneAsync(item)
        .ContinueWith((task) => item, TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public virtual Task<TItem> UpdateItem(TItem item, string user) => GetCollection()
        .FindOneAndReplaceAsync(Builders<TItem>.Filter.Eq(nameof(DatabaseObject.Id), item.Id), item, new() { ReturnDocument = ReturnDocument.After })
        .ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(item)), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public virtual Task<TItem> DeleteItem(string id, string user) => GetCollection()
        .FindOneAndDeleteAsync(Builders<TItem>.Filter.Eq(nameof(DatabaseObject.Id), id))
        .ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(id)), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <summary>
    /// Remove all content.
    /// </summary>
    /// <returns>Conroller of the outstanding operation.</returns>
    public Task<long> RemoveAll() => GetCollection<BsonDocument>()
        .DeleteManyAsync(FilterDefinition<BsonDocument>.Empty)
        .ContinueWith(t => t.Result.DeletedCount, TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable() => GetCollection().AsQueryable();
}

