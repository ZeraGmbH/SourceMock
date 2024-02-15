using MongoDB.Bson;
using MongoDB.Driver;
using SharedLibrary.Models;
using SharedLibrary.Services;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// Database managed counters.
/// </summary>
public class MongoDbCounters : ICounterCollection
{
    private readonly IMongoDbDatabaseService _database;

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="database">Database to use</param>
    public MongoDbCounters(IMongoDbDatabaseService database)
    {
        _database = database;
    }

    /// <inheritdoc/>
    public async Task<long> GetNextCounter(string name)
    {
        /* Attach to connection. */
        var collection = _database.Database.GetCollection<BsonDocument>("sam-sequence-numbers");

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

