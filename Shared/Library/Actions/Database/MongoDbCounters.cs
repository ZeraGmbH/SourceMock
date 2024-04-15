using MongoDB.Bson;
using MongoDB.Driver;
using SharedLibrary.Models;
using SharedLibrary.Services;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// Database managed counters.
/// </summary>
sealed class MongoDbCounters : ICounterCollection
{
    private readonly IMongoDbDatabaseService _database;

    private readonly string _category;

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="database">Database to use</param>
    /// <param name="category">Category of the database.</param>
    public MongoDbCounters(string category, IMongoDbDatabaseService database)
    {
        _database = database;
        _category = category;
    }

    /// <inheritdoc/>
    public async Task<long> GetNextCounter(string name)
    {
        /* Attach to connection. */
        var collection = _database.GetDatabase(_category).GetCollection<BsonDocument>("sam-sequence-numbers");

        /* Update the counter - eventually create a new master object. */
        var updated = await collection.FindOneAndUpdateAsync<BsonDocument>(
            Builders<BsonDocument>.Filter.Eq("_id", 1),
            Builders<BsonDocument>.Update.SetOnInsert("_id", 1).Inc(name, 1),
            new() { IsUpsert = true, ReturnDocument = ReturnDocument.After }
        )!;

        /* Report new counter. */
        return updated[name].ToInt64();
    }
}


/// <summary>
/// 
/// </summary>
/// <param name="_database"></param>
public class MongoDbCountersFactory(IMongoDbDatabaseService _database) : ICounterCollectionFactory
{
    /// <inheritdoc/>
    public ICounterCollection Create(string category) => new MongoDbCounters(category, _database);
}
