
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
/// <typeparam name="TSingleton"></typeparam>
/// <param name="collectionName"></param>
/// <param name="category">Category of the database.</param>
/// <param name="singleton"></param>
/// <param name="_database">Database connection to use.</param>
sealed class MongoDbCollection<TItem, TSingleton>(string collectionName, string category, TSingleton singleton, IMongoDbDatabaseService _database) : IObjectCollection<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    public TSingleton Common => singleton;

    /// <summary>
    /// Name of the collection to use.
    /// </summary>
    public readonly string CollectionName = collectionName;

    private IMongoCollection<T> GetCollection<T>() => _database.GetDatabase(category).GetCollection<T>(CollectionName);

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

    private static readonly Collation _noCase = new("en", strength: CollationStrength.Secondary);

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
/// <typeparam name="TSingleton"></typeparam>
/// <param name="database"></param>
/// <param name="singleton"></param>
public class MongoDbCollectionFactory<TItem, TSingleton>(IMongoDbDatabaseService database, TSingleton singleton) : IObjectCollectionFactory<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IObjectCollection<TItem, TSingleton> Create(string uniqueName, string category)
    {
        var collection = new MongoDbCollection<TItem, TSingleton>(uniqueName, category, singleton, database);

        singleton.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <param name="database"></param>
public class MongoDbCollectionFactory<TItem>(IMongoDbDatabaseService database) : MongoDbCollectionFactory<TItem, NoopInitializer<TItem>>(database, new()), IObjectCollectionFactory<TItem> where TItem : IDatabaseObject
{
}

