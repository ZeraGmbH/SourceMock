
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;


/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
/// <typeparam name="TInitializer"></typeparam>
/// <param name="_onetimeInitializer"></param>
public sealed class InMemoryFiles<TItem, TInitializer>(InMemoryFiles<TItem, TInitializer>.State _onetimeInitializer) : IFileCollection<TItem, TInitializer>
    where TInitializer : IFileInitializer<TItem>
{
    /// <summary>
    /// 
    /// </summary>
    public class Item : FileInfo<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public class StateFactory(IServiceProvider services)
    {
        private readonly ConcurrentDictionary<string, State> _map = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public State GetOrAdd(string key) => _map.GetOrAdd(key, _ => new State(services.GetRequiredService<TInitializer>()));
    }

    /// <summary>
    /// 
    /// </summary>
    public class State(TInitializer _onetimeInitializer) : FileInitializer<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly TInitializer OnetimeInitializer = _onetimeInitializer;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Item> Data { get; private set; } = [];

        /// <inheritdoc/>
        protected override Task OnInitialize(IFileCollection<TItem> collection) => OnetimeInitializer.Initialize(collection);
    }

    /// <summary>
    /// 
    /// </summary>
    public TInitializer Common => _onetimeInitializer.OnetimeInitializer;

    /// <summary>
    /// Clone item using the standard InMemory serialisation mechanisms. 
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <returns>Clone of the item.</returns>
    public Item CloneFile(Item item)
    {
        /* Make us behave just like real implementations will do. */
        using var writer = new BsonDocumentWriter([]);

        BsonSerializer.Serialize(writer, item);

        return BsonSerializer.Deserialize<Item>(writer.Document);
    }

    /// <inheritdoc/>
    public Task<string> AddFile(string name, TItem meta, Stream content)
    {
        /* Always create a fully detached clone. */
        var data = new byte[content.Length];

        content.Read(data);

        var clone = CloneFile(new Item
        {
            Data = data,
            Id = Guid.NewGuid().ToString(),
            Length = data.Length,
            Meta = meta,
            Name = name,
            UploadedAt = DateTime.Now,
        });

        /* Add to dictionary. */
        lock (_onetimeInitializer.Data)
            _onetimeInitializer.Data.Add(clone.Id, clone);

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(clone.Id);
    }

    /// <inheritdoc/>
    public Task DeleteFile(string id)
    {
        /* Remove from dictionary. */
        lock (_onetimeInitializer.Data)
            _onetimeInitializer.Data.Remove(id);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAll()
    {
        lock (_onetimeInitializer.Data)
            _onetimeInitializer.Data.Clear();

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<FileInfo<TItem>?> FindFile(string id)
    {
        lock (_onetimeInitializer.Data)
            return Task.FromResult<FileInfo<TItem>?>(_onetimeInitializer.Data.TryGetValue(id, out var file)
                ? CloneFile(file)
                : null
            );
    }

    /// <inheritdoc/>
    public Task<Stream> Open(string id)
    {
        lock (_onetimeInitializer.Data)
            return Task.FromResult<Stream>(new MemoryStream(_onetimeInitializer.Data[id].Data));
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TInitializer"></typeparam>
public class InMemoryFilesFactory<TItem, TInitializer>(InMemoryFiles<TItem, TInitializer>.StateFactory factory) : IFileCollectionFactory<TItem, TInitializer>
    where TInitializer : IFileInitializer<TItem>
{
    /// <inheritdoc/>
    public IFileCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var initializer = factory.GetOrAdd($"{category}:{uniqueName}");
        var collection = new InMemoryFiles<TItem, TInitializer>(initializer);

        initializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryFilesFactory<TItem>(InMemoryFiles<TItem, NoopFileInitializer<TItem>>.StateFactory factory) : InMemoryFilesFactory<TItem, NoopFileInitializer<TItem>>(factory), IFileCollectionFactory<TItem>
{
}
