
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedLibrary.Models;
using SharedLibrary.Services;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// MongoDb collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
/// <param name="collectionName"></param>
/// <param name="_database">Database connection to use.</param>
public sealed class MongoDbCollection<TItem>(string collectionName, IMongoDbDatabaseService _database) : IObjectCollection<TItem> where TItem : IDatabaseObject
{
    /// <summary>
    /// Name of the collection to use.
    /// </summary>
    public readonly string CollectionName = collectionName;

    private IMongoCollection<T> GetCollection<T>() => _database.GetDatabase().GetCollection<T>(CollectionName);

    private IMongoCollection<TItem> GetCollection() => GetCollection<TItem>();

    /// <inheritdoc/>
    public Task<TItem> AddItem(TItem item, string user) => GetCollection()
        .InsertOneAsync(item)
        .ContinueWith((task) => item, TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item, string user) => GetCollection()
        .FindOneAndReplaceAsync(Builders<TItem>.Filter.Eq(nameof(IDatabaseObject.Id), item.Id), item, new() { ReturnDocument = ReturnDocument.After })
        .ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(item)), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, string user, bool silent = false) => GetCollection()
        .FindOneAndDeleteAsync(Builders<TItem>.Filter.Eq(nameof(IDatabaseObject.Id), id))
        .ContinueWith(t => t.Result ?? (silent ? default(TItem)! : throw new ArgumentException("item not found", nameof(id))), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <summary>
    /// Remove all content.
    /// </summary>
    /// <returns>Conroller of the outstanding operation.</returns>
    public Task<long> RemoveAll() => GetCollection<BsonDocument>()
        .DeleteManyAsync(FilterDefinition<BsonDocument>.Empty)
        .ContinueWith(t => t.Result.DeletedCount, TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable() => GetCollection().AsQueryable();

    private readonly Collation _noCase = new("en", strength: CollationStrength.Secondary);

    /// <inheritdoc/>
    public Task<string> CreateIndex(string name, Expression<Func<TItem, object>> keyAccessor, bool ascending = true, bool unique = true, bool caseSensitive = true)
    {
        var builder = Builders<TItem>.IndexKeys;
        var keys = ascending ? builder.Ascending(keyAccessor) : builder.Descending(keyAccessor);

        return GetCollection<TItem>()
            .Indexes
            .CreateOneAsync(new CreateIndexModel<TItem>(
                keys,
                new CreateIndexOptions { Name = name, Collation = caseSensitive ? null : _noCase, Unique = unique, }
            ));
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <remarks>
/// 
/// </remarks>
/// <param name="_database"></param>
public class MongoDbCollectionFactory<TItem>(IMongoDbDatabaseService _database) : IObjectCollectionFactory<TItem> where TItem : IDatabaseObject
{
    /// <inheritdoc/>
    public IObjectCollection<TItem> Create(string uniqueName) => new MongoDbCollection<TItem>(uniqueName, _database);
}

