using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SharedLibrary.Actions.User;
using SharedLibrary.Models;
using SharedLibrary.Services;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
/// <param name="_bucketName"></param>
/// <param name="_category"></param>
/// <param name="_onetimeInitializer"></param>
/// <param name="_database"></param>
/// <param name="_user"></param>
sealed class MongoDbFiles<TItem, TInitializer>(string _bucketName, string _category, TInitializer _onetimeInitializer, IMongoDbDatabaseService _database, ICurrentUser _user) : IFilesCollection<TItem, TInitializer>
    where TInitializer : IFilesInitializer<TItem>
{
    private const string UserId = "__userId__";

    /// <summary>
    /// 
    /// </summary>
    public TInitializer Common => _onetimeInitializer;

    private readonly GridFSBucket _bucket = new(_database.GetDatabase(_category), new() { BucketName = _bucketName });

    /// <inheritdoc/>
    public async Task<string> AddFile(string name, TItem meta, Stream content)
    {
        var dbMeta = meta.ToBsonDocument();

        dbMeta[UserId] = _user.GetUserId();

        var id = await _bucket.UploadFromStreamAsync(name, content, new GridFSUploadOptions() { Metadata = dbMeta });

        return id.ToString();
    }

    /// <inheritdoc/>
    public async Task<FileInfo<TItem>?> FindFile(string id)
    {
        var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(id)));
        var file = await files.SingleOrDefaultAsync();

        if (file == null) return default;

        var rawMeta = file.Metadata;
        var userId = rawMeta[UserId].AsString;

        rawMeta.Remove(UserId);

        return new()
        {
            Id = file.Id.ToString(),
            Length = file.Length,
            Meta = BsonSerializer.Deserialize<TItem>(rawMeta),
            Name = file.Filename,
            UploadedAt = file.UploadDateTime,
            UserId = userId,
        };
    }

    /// <inheritdoc/>
    public Task RemoveAll() => _bucket.DropAsync();

    /// <inheritdoc/>
    public Task DeleteFile(string id) => _bucket.DeleteAsync(ObjectId.Parse(id));

    /// <inheritdoc/>
    public async Task<Stream> Open(string id) => await _bucket.OpenDownloadStreamAsync(ObjectId.Parse(id));
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
/// <param name="database"></param>
/// <param name="onetimeInitializer"></param>
/// <param name="user"></param>
public class MongoDbFilesCollectionFactory<TItem, TInitializer>(IMongoDbDatabaseService database, TInitializer onetimeInitializer, ICurrentUser user) : IFilesCollectionFactory<TItem, TInitializer>
    where TInitializer : IFilesInitializer<TItem>
{
    /// <inheritdoc/>
    public IFilesCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var collection = new MongoDbFiles<TItem, TInitializer>(uniqueName, category, onetimeInitializer, database, user);

        onetimeInitializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <param name="database"></param>
/// <param name="user"></param>
public class MongoDbFilesCollectionFactory<TItem>(IMongoDbDatabaseService database, ICurrentUser user) : MongoDbFilesCollectionFactory<TItem, NoopFilesInitializer<TItem>>(database, new(), user), IFilesCollectionFactory<TItem>
{
}