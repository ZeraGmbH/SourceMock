
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
/// <typeparam name="TInitializer"></typeparam>
/// <param name="_onetimeInitializer"></param>
public sealed class InMemoryCollection<TItem, TInitializer>(InMemoryCollection<TItem, TInitializer>.State _onetimeInitializer) : IObjectCollection<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
{
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
    public class State(TInitializer _onetimeInitializer) : CollectionInitializer<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly TInitializer OnetimeInitializer = _onetimeInitializer;

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
        protected override Task OnInitialize(IObjectCollection<TItem> collection) => OnetimeInitializer.Initialize(collection);
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
        lock (_onetimeInitializer.Data)
        {
            if (_onetimeInitializer.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_onetimeInitializer.Data.TryAdd(clone.Id, clone))
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
        lock (_onetimeInitializer.Data)
        {
            if (_onetimeInitializer.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_onetimeInitializer.Data.ContainsKey(item.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            _onetimeInitializer.Data[clone.Id] = clone;
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, bool silent = false)
    {
        /* Remove from dictionary. */
        lock (_onetimeInitializer.Data)
        {
            if (!_onetimeInitializer.Data.TryGetValue(id, out var clone))
                return silent ? Task.FromResult(default(TItem)!) : Task.FromException<TItem>(new ArgumentException("item not found", nameof(id)));

            _onetimeInitializer.Data.Remove(id);

            return Task.FromResult(clone);
        }
    }

    /// <inheritdoc/>
    public Task<long> RemoveAll()
    {
        lock (_onetimeInitializer.Data)
            try
            {
                return Task.FromResult<long>(_onetimeInitializer.Data.Count);
            }
            finally
            {
                _onetimeInitializer.Data.Clear();
            }
    }

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable()
    {
        lock (_onetimeInitializer.Data)
            return _onetimeInitializer.Data.Values.Select(CloneItem).ToArray().AsQueryable();
    }

    /// <inheritdoc/>
    public Task<string> CreateIndex(string name, Expression<Func<TItem, object>> keyAccessor, bool ascending = true, bool unique = true, bool caseSensitive = true)
    {
        if (unique)
        {
            var getKey = keyAccessor.Compile();

            _onetimeInitializer.IndexCheck += (newItem) =>
            {
                var newValue = (string)getKey.Invoke(newItem);

                if (!caseSensitive) newValue = newValue?.ToLowerInvariant();

                var match = _onetimeInitializer.Data.Values.SingleOrDefault(i =>
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
/// <typeparam name="TInitializer"></typeparam>
public class InMemoryCollectionFactory<TItem, TInitializer>(InMemoryCollection<TItem, TInitializer>.StateFactory factory) : IObjectCollectionFactory<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IObjectCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var initializer = factory.GetOrAdd($"{category}:{uniqueName}");
        var collection = new InMemoryCollection<TItem, TInitializer>(initializer);

        initializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryCollectionFactory<TItem>(InMemoryCollection<TItem, NoopInitializer<TItem>>.StateFactory factory) : InMemoryCollectionFactory<TItem, NoopInitializer<TItem>>(factory), IObjectCollectionFactory<TItem>
    where TItem : IDatabaseObject
{
}
