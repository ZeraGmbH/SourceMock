
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;


/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
/// <typeparam name="TSingleton"></typeparam>
/// <param name="singleton"></param>
public sealed class InMemoryCollection<TItem, TSingleton>(InMemoryCollection<TItem, TSingleton>.Initializer singleton) : IObjectCollection<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public class InitializerFactory(IServiceProvider services)
    {
        private readonly ConcurrentDictionary<string, Initializer> _map = [];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Initializer GetOrAdd(string key) => _map.GetOrAdd(key, _ => new Initializer(services.GetRequiredService<TSingleton>()));
    }

    /// <summary>
    /// 
    /// </summary>
    public class Initializer(TSingleton singleton) : CollectionInitializer<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly TSingleton Singleton = singleton;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, TItem> Data { get; private set; } = [];

        /// <summary>
        /// 
        /// </summary>
        public event Func<TItem, bool>? IndexCheck = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool? CheckIndex(TItem item) => IndexCheck?.Invoke(item);

        /// <inheritdoc/>
        protected override Task OnInitialize(IObjectCollection<TItem> collection) => Singleton.Initialize(collection);
    }

    /// <summary>
    /// 
    /// </summary>
    public TSingleton Common => singleton.Singleton;

    /// <summary>
    /// Clone item using the standard InMemory serialisation mechanisms. 
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <returns>Clone of the item.</returns>
    public TItem CloneItem(TItem item)
    {
        /* Make us behave just like real implementations will do. */
        using var writer = new BsonDocumentWriter(new BsonDocument());

        BsonSerializer.Serialize(writer, item);

        return BsonSerializer.Deserialize<TItem>(writer.Document);
    }

    /// <inheritdoc/>
    public Task<TItem> AddItem(TItem item)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Add to dictionary. */
        lock (singleton.Data)
        {
            if (singleton.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!singleton.Data.TryAdd(clone.Id, clone))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Replace in dictionary. */
        lock (singleton.Data)
        {
            if (singleton.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!singleton.Data.ContainsKey(item.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            singleton.Data[clone.Id] = clone;
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, bool silent = false)
    {
        /* Remove from dictionary. */
        lock (singleton.Data)
        {
            if (!singleton.Data.TryGetValue(id, out var clone))
                return silent ? Task.FromResult(default(TItem)!) : Task.FromException<TItem>(new ArgumentException("item not found", nameof(id)));

            singleton.Data.Remove(id);

            return Task.FromResult(clone);
        }
    }

    /// <inheritdoc/>
    public Task<long> RemoveAll()
    {
        lock (singleton.Data)
            try
            {
                return Task.FromResult<long>(singleton.Data.Count);
            }
            finally
            {
                singleton.Data.Clear();
            }
    }

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable()
    {
        lock (singleton.Data)
            return singleton.Data.Values.Select(CloneItem).ToArray().AsQueryable();
    }

    /// <inheritdoc/>
    public Task<string> CreateIndex(string name, Expression<Func<TItem, object>> keyAccessor, bool ascending = true, bool unique = true, bool caseSensitive = true)
    {
        if (unique)
        {
            var getKey = keyAccessor.Compile();

            singleton.IndexCheck += (newItem) =>
            {
                var newValue = (string)getKey.Invoke(newItem);

                if (!caseSensitive) newValue = newValue?.ToLowerInvariant();

                var match = singleton.Data.Values.SingleOrDefault(i =>
                {
                    var oldValue = (string)getKey.Invoke(i);

                    if (!caseSensitive) oldValue = oldValue?.ToLowerInvariant();

                    return oldValue == newValue;
                });

                if (match == null) return true;

                return match.Id == newItem.Id;
            };
        }

        return Task.FromResult(name);
    }
}


/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TSingleton"></typeparam>
public class InMemoryCollectionFactory<TItem, TSingleton>(InMemoryCollection<TItem, TSingleton>.InitializerFactory factory) : IObjectCollectionFactory<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IObjectCollection<TItem, TSingleton> Create(string uniqueName, string category)
    {
        var initializer = factory.GetOrAdd($"{category}:{uniqueName}");
        var collection = new InMemoryCollection<TItem, TSingleton>(initializer);

        initializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryCollectionFactory<TItem>(InMemoryCollection<TItem, NoopInitializer<TItem>>.InitializerFactory factory) : InMemoryCollectionFactory<TItem, NoopInitializer<TItem>>(factory), IObjectCollectionFactory<TItem>
    where TItem : IDatabaseObject
{
}
