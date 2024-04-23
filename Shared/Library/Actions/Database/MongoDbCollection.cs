
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
/// <typeparam name="TInitializer"></typeparam>
/// <param name="_collectionName"></param>
/// <param name="_category">Category of the database.</param>
/// <param name="_onetimeInitializer"></param>
/// <param name="_database">Database connection to use.</param>
sealed class MongoDbCollection<TItem, TInitializer>(
    string _collectionName,
    string _category,
    TInitializer _onetimeInitializer,
    IMongoDbDatabaseService _database
) : IObjectCollection<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
{
    public TInitializer Common => _onetimeInitializer;

    /// <summary>
    /// Name of the collection to use.
    /// </summary>
    public readonly string CollectionName = _collectionName;

    private IMongoCollection<T> GetCollection<T>() => _database.GetDatabase(_category).GetCollection<T>(CollectionName);

    private IMongoCollection<TItem> GetCollection() => GetCollection<TItem>();

    /// <inheritdoc/>
    public Task<TItem> AddItem(TItem item) => GetCollection()
        .InsertOneAsync(item)
        .ContinueWith((task) => item, TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item) => GetCollection()
        .FindOneAndReplaceAsync(Builders<TItem>.Filter.Eq(nameof(IDatabaseObject.Id), item.Id), item, new() { ReturnDocument = ReturnDocument.After })
        .ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(item)), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, bool silent = false) => GetCollection()
        .FindOneAndDeleteAsync(Builders<TItem>.Filter.Eq(nameof(IDatabaseObject.Id), id))
        .ContinueWith(t => t.Result ?? (silent ? default(TItem)! : throw new ArgumentException("item not found", nameof(id))), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public Task<long> DeleteItems(Expression<Func<TItem, bool>> filter) => GetCollection()
        .DeleteManyAsync(Builders<TItem>.Filter.Where(filter))
        .ContinueWith(t => t.Result.DeletedCount);

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
/// <typeparam name="TInitializer"></typeparam>
/// <param name="database"></param>
/// <param name="onetimeInitializer"></param>
public class MongoDbCollectionFactory<TItem, TInitializer>(IMongoDbDatabaseService database, TInitializer onetimeInitializer) : IObjectCollectionFactory<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IObjectCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var collection = new MongoDbCollection<TItem, TInitializer>(uniqueName, category, onetimeInitializer, database);

        onetimeInitializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <param name="database"></param>
public class MongoDbCollectionFactory<TItem>(IMongoDbDatabaseService database) : MongoDbCollectionFactory<TItem, NoopCollectionInitializer<TItem>>(database, new()), IObjectCollectionFactory<TItem> where TItem : IDatabaseObject
{
}

