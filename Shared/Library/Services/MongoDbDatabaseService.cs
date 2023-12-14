using MongoDB.Driver;
using SharedLibrary.Models;

namespace SharedLibrary.Services;

/// <summary>
/// Interface provided by each database service.
/// </summary>
public interface IMongoDbDatabaseService
{
    /// <summary>
    /// MongoDB datbase reference for this service.
    /// </summary>
    IMongoDatabase Database { get; }
}

/// <summary>
/// Connection to a single database.
/// </summary>
public class MongoDbDatabaseService : IMongoDbDatabaseService
{
    private readonly IMongoDatabase _database;

    /// <summary>
    /// Underlying MongoDb database reference.
    /// </summary>
    public IMongoDatabase Database => _database;

    /// <summary>
    /// Initialize a new database connection.
    /// </summary>
    /// <param name="settings">Configuration to access the MongoDb database.</param>
    public MongoDbDatabaseService(MongoDbSettings settings)
    {
        var config = new MongoClientSettings { Server = new MongoServerAddress(settings.ServerName) };

        if (!string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
            config.Credential = MongoCredential.CreateCredential(settings.DatabaseName, settings.UserName, settings.Password);

        var client = new MongoClient(config);

        _database = client.GetDatabase(settings.DatabaseName);
    }
}