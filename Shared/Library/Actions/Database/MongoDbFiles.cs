using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
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
sealed class MongoDbFiles<TItem, TInitializer>(string _bucketName, string _category, TInitializer _onetimeInitializer, IMongoDbDatabaseService _database) : IFileCollection<TItem, TInitializer>
    where TInitializer : IFileInitializer<TItem>
{
    /// <summary>
    /// 
    /// </summary>
    public TInitializer Common => _onetimeInitializer;

    private readonly GridFSBucket _bucket = new(_database.GetDatabase(_category), new() { BucketName = _bucketName });

    /// <inheritdoc/>
    public async Task<string> AddFile(string name, TItem meta, Stream content)
    {
        var id = await _bucket.UploadFromStreamAsync(name, content, new GridFSUploadOptions() { Metadata = meta.ToBsonDocument() });

        return id.ToString();
    }

    /// <inheritdoc/>
    public async Task<FileInfo<TItem>?> FindFile(string id)
    {
        var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", ObjectId.Parse(id)));
        var file = await files.SingleOrDefaultAsync();

        if (file == null) return default;

        return new()
        {
            Id = file.Id.ToString(),
            Length = file.Length,
            Meta = BsonSerializer.Deserialize<TItem>(file.Metadata),
            Name = file.Filename,
            UploadedAt = file.UploadDateTime,
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
public class MongoDbFileFactory<TItem, TInitializer>(IMongoDbDatabaseService database, TInitializer onetimeInitializer) : IFileCollectionFactory<TItem, TInitializer>
    where TInitializer : IFileInitializer<TItem>
{
    /// <inheritdoc/>
    public IFileCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var collection = new MongoDbFiles<TItem, TInitializer>(uniqueName, category, onetimeInitializer, database);

        onetimeInitializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <param name="database"></param>
public class MongoDbFileFactory<TItem>(IMongoDbDatabaseService database) : MongoDbFileFactory<TItem, NoopFileInitializer<TItem>>(database, new()), IFileCollectionFactory<TItem>
{
}