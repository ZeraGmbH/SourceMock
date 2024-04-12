using System.Collections.Concurrent;
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
    /// <param name="category">Category name of the database - to support multiple databases.</param>
    IMongoDatabase GetDatabase(string category);
}

/// <summary>
/// Database connection managaer.
/// </summary>
public class MongoDbDatabaseService : IMongoDbDatabaseService
{
    private readonly ConcurrentDictionary<string, IMongoDatabase> _databases = [];

    private readonly Dictionary<string, MongoDbSettings> _categories = [];

    /// <summary>
    /// Underlying MongoDb database reference.
    /// </summary>
    public IMongoDatabase GetDatabase(string category)
    {
        /* Category must be pre-defined. */
        var settings = _categories[category];

        return _databases.GetOrAdd(category, c =>
        {
            /* Create client configuration from server configuration. */
            var config = new MongoClientSettings { Server = new MongoServerAddress(settings.ServerName) };

            if (!string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
                config.Credential = MongoCredential.CreateCredential(settings.DatabaseName, settings.UserName, settings.Password);

            /* Create the client. */
            var client = new MongoClient(config);

            /* Create accessor to the database. */
            return client.GetDatabase(settings.DatabaseName);
        });
    }

    /// <summary>
    /// Register a database for a category.
    /// </summary>
    /// <param name="category">Name of the category.</param>
    /// <param name="settings">Configuration to use.</param>
    public void RegisterCategory(string category, MongoDbSettings settings) => _categories.Add(category, settings);

    /// <summary>
    /// Initialize new database manager.
    /// </summary>
    /// <param name="masterDatabaseSettings">Configuration to access the primary database.</param>
    public MongoDbDatabaseService(MongoDbSettings masterDatabaseSettings)
    {
        /* For now configure all well known categories to use the same database. */
        foreach (var field in typeof(DatabaseCategories).GetFields())
            if (field.IsLiteral)
                RegisterCategory((string)field.GetValue(null)!, masterDatabaseSettings);
    }
}