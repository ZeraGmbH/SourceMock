
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using DeviceApiSharedLibrary.Models;
using DeviceApiSharedLibrary.Services;
using DeviceApiLib.Actions.Database;

namespace DeviceApiSharedLibrary.Actions.Database;

/// <summary>
/// MongoDb collection with automatic document history.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
public sealed class MongoDbHistoryCollection<TItem> : IHistoryCollection<TItem> where TItem : IDatabaseObject
{
    /// <summary>
    /// Field added to each item containing history information.
    /// </summary>
    private const string HistoryField = "_history";

    private const string HistoryVersionField = "version";

    private static readonly string HistoryVersionPath = $"{HistoryField}.{HistoryVersionField}";

    /// <summary>
    /// Name of the collection to use.
    /// </summary>
    public readonly string CollectionName;

    private string HistoryCollectionName => $"{CollectionName}-history";

    private readonly IMongoDbDatabaseService _database;

    private readonly ILogger<MongoDbHistoryCollection<TItem>> _logger;

    /// <summary>
    /// Initializes a new collection.
    /// </summary>
    /// <param name="database">Database connection to use.</param>
    /// <param name="logger">Logging instance to use.</param>
    public MongoDbHistoryCollection(string collectionName, IMongoDbDatabaseService database, ILogger<MongoDbHistoryCollection<TItem>> logger)
    {
        CollectionName = collectionName;

        _database = database;
        _logger = logger;
    }

    public IMongoCollection<T> GetCollection<T>() => _database.Database.GetCollection<T>(CollectionName);

    public IMongoCollection<T> GetHistoryCollection<T>() => _database.Database.GetCollection<T>(HistoryCollectionName);

    /// <inheritdoc/>
    public Task<TItem> AddItem(TItem item, string user)
    {
        /* Create all fields used for historisation. */
        var now = DateTime.Now;
        var history = new HistoryInfo { ChangeCount = 1, CreatedAt = now, CreatedBy = user, ModifiedAt = now, ModifiedBy = user };

        /* Convert item to document and add historisation data. */
        var doc = item.ToBsonDocument();

        doc[HistoryField] = history.ToBsonDocument();

        /* Add item to the database an report a new item instance. */
        return GetCollection<BsonDocument>()
            .InsertOneAsync(doc)
            .ContinueWith((task) => BsonSerializer.Deserialize<TItem>(doc), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    /// <inheritdoc/>
    public async Task<TItem> UpdateItem(TItem item, string user)
    {
        /* Apply history information. */
        var doc = item.ToBsonDocument();

        doc[$"{HistoryField}.modifiedAt"] = DateTime.Now;
        doc[$"{HistoryField}.modifiedBy"] = user;

        var id = doc["_id"].ToString();

        /* Update the item itself. */
        var self = GetCollection<BsonDocument>();

        var previous = await self
            .FindOneAndUpdateAsync(
                new BsonDocument { { "_id", id } },
                new BsonDocument {
                    { "$set", doc },
                    { "$inc", new BsonDocument{ { HistoryVersionPath, 1 } } }
                },
                new FindOneAndUpdateOptions<BsonDocument, BsonDocument> { ReturnDocument = ReturnDocument.Before }
            ).ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(item)));

        /* Then apply the history entry, */
        await GetHistoryCollection<BsonDocument>().InsertOneAsync(new BsonDocument{
            { "_id", Guid.NewGuid().ToString() },
            { "item", previous }
        });

        /* Must requery to get the current item - especially with the update version number. */
        return await GetCollection<TItem>()
            .FindAsync(Builders<TItem>.Filter.Eq(nameof(IDatabaseObject.Id), id))
            .ContinueWith(task => task.Result.SingleOrDefault(), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    /// <inheritdoc/>
    public async Task<TItem> DeleteItem(string id, string user)
    {
        var self = GetCollection<BsonDocument>();

        var previous = await self
            .FindOneAndUpdateAsync(
                new BsonDocument { { "_id", id } },
                new BsonDocument {
                    { "$set", new BsonDocument {
                        { $"{HistoryField}.modifiedAt", DateTime.Now },
                        { $"{HistoryField}.modifiedBy", user } }
                    },
                    { "$inc", new BsonDocument{ { HistoryVersionPath, 1 } } }
                },
                new FindOneAndUpdateOptions<BsonDocument, BsonDocument> { ReturnDocument = ReturnDocument.Before }
            ).ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(id)));

        await GetHistoryCollection<BsonDocument>().InsertOneAsync(new BsonDocument{
            { "_id", Guid.NewGuid().ToString() },
            { "item", previous }
        });

        var deleted = await self
            .FindOneAndDeleteAsync(new BsonDocument { { "_id", id } })
            .ContinueWith(t => t.Result ?? throw new ArgumentException("item not found", nameof(id)));

        await GetHistoryCollection<BsonDocument>().InsertOneAsync(new BsonDocument {
            { "_id", Guid.NewGuid().ToString() },
            { "item", deleted }
        });

        return BsonSerializer.Deserialize<TItem>(deleted);
    }

    /// <summary>
    /// Find items and add the full history information.
    /// </summary>
    /// <param name="id">Primary key of the document the history should be reported.</param>
    /// <returns>History of the document sorted by version descending - i.e. newest first.</returns>
    public Task<IEnumerable<HistoryItem<TItem>>> GetHistory(string id)
    {
        var pipeline = PipelineDefinitionBuilder
            /* Use raw BSON semantic to be more flexible on operations. */
            .For<BsonDocument>()
            /* Filter on all related history items and use these as the new document stream.. */
            .Match(new BsonDocument { { "item._id", id } })
            .ReplaceRoot<BsonDocument, BsonDocument, BsonDocument>("$item")
            /* Add the current item itself - up to now we have only history entries. */
            .UnionWith(GetCollection<BsonDocument>(), PipelineDefinitionBuilder
                .For<BsonDocument>()
                .Match(new BsonDocument { { "_id", id } })
            )
            /* Newest version first - if existing this will start with the current item. */
            .Sort(new BsonDocument { { "_id", 1 }, { HistoryVersionPath, -1 } })
            /* Make it look like all history items are regular items - the _id are provided by the documents themselves.. */
            .Project(new BsonDocument {
                { "_id", 0 },
                { "Item", "$$ROOT" },
                { "Version", $"${HistoryField}" },
            });

        /* Execute the pipeline and recreated the items. */
        return GetHistoryCollection<BsonDocument>()
            .Aggregate(pipeline)
            .ToListAsync()
            .ContinueWith((task) => task.Result.Select(doc => BsonSerializer.Deserialize<HistoryItem<TItem>>(doc)), TaskContinuationOptions.OnlyOnRanToCompletion);
    }

    /// <summary>
    /// Remove all content.
    /// </summary>
    /// <returns>Conroller of the outstanding operation.</returns>
    public Task<long> RemoveAll() =>
       Task.WhenAll(
           GetCollection<BsonDocument>().DeleteManyAsync(FilterDefinition<BsonDocument>.Empty),
           GetHistoryCollection<BsonDocument>().DeleteManyAsync(FilterDefinition<BsonDocument>.Empty)
       ).ContinueWith(tasks => tasks.Result.Sum(r => r.DeletedCount), TaskContinuationOptions.OnlyOnRanToCompletion);

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable() => GetCollection<TItem>().AsQueryable();
}


/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class MongoDbHistoryCollectionFactory<TItem> : IHistoryCollectionFactory<TItem> where TItem : IDatabaseObject
{
    private readonly ILogger<MongoDbHistoryCollection<TItem>> _logger;

    private readonly IMongoDbDatabaseService _database;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public MongoDbHistoryCollectionFactory(IMongoDbDatabaseService database, ILogger<MongoDbHistoryCollection<TItem>> logger)
    {
        _database = database;
        _logger = logger;
    }

    /// <inheritdoc/>
    public IHistoryCollection<TItem> Create(string uniqueName) => new MongoDbHistoryCollection<TItem>(uniqueName, _database, _logger);
}

