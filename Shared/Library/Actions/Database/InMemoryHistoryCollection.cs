using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharedLibrary.Actions.User;
using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
/// <typeparam name="TInitializer"></typeparam>
/// <param name="_onetimeInitializer"></param>
/// <param name="_user"></param>
public sealed class InMemoryHistoryCollection<TItem, TInitializer>(InMemoryHistoryCollection<TItem, TInitializer>.Initializer _onetimeInitializer, ICurrentUser _user) : IHistoryCollection<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
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
        public Initializer GetOrAdd(string key) => _map.GetOrAdd(key, _ => new Initializer(services.GetRequiredService<TInitializer>()));
    }

    /// <summary>
    /// 
    /// </summary>
    public class Initializer(TInitializer _onetimeInitializer) : CollectionInitializer<TItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public readonly TInitializer OnetimeInitializer = _onetimeInitializer;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, List<HistoryItem<TItem>>> Data = [];

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected override Task OnInitialize(IObjectCollection<TItem> collection) => OnetimeInitializer.Initialize(collection);
    }

    /// <summary>
    /// 
    /// </summary>
    public TInitializer Common => _onetimeInitializer.OnetimeInitializer;

    /// <summary>
    /// Clone item using the standard MongoDb serialisation mechanisms. 
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <typeparam name="T">Type of the item to clone.</typeparam>
    /// <returns>Clone of the item.</returns>
    public static T CloneItem<T>(T item)
    {
        /* Make us behave just like real implementations will do. */
        using var writer = new BsonDocumentWriter([]);

        BsonSerializer.Serialize(writer, item);

        return BsonSerializer.Deserialize<T>(writer.Document);
    }

    /// <inheritdoc/>
    public Task<TItem> AddItem(TItem item)
    {
        /* Create all fields used for historisation. */
        var now = DateTime.Now;
        var userId = _user.GetUserId();

        /* Always create a fully detached clone. */
        var clone = new HistoryItem<TItem>
        {
            Item = CloneItem(item),
            Version = new HistoryInfo
            {
                ChangeCount = 1,
                CreatedAt = now,
                CreatedBy = userId,
                ModifiedAt = now,
                ModifiedBy = userId
            }
        };

        /* Add to dictionary. */
        lock (_onetimeInitializer.Data)
        {
            if (_onetimeInitializer.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_onetimeInitializer.Data.TryAdd(clone.Item.Id, [clone]))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone.Item));
    }

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item)
    {
        var now = DateTime.Now;

        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Replace in dictionary. */
        lock (_onetimeInitializer.Data)
        {
            if (_onetimeInitializer.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_onetimeInitializer.Data.ContainsKey(clone.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            var list = _onetimeInitializer.Data[clone.Id];
            var previous = list[^1];

            list.Add(new HistoryItem<TItem>
            {
                Item = clone,
                Version = new HistoryInfo
                {
                    ChangeCount = previous.Version.ChangeCount + 1,
                    CreatedAt = previous.Version.CreatedAt,
                    CreatedBy = previous.Version.CreatedBy,
                    ModifiedAt = now,
                    ModifiedBy = _user.GetUserId()
                }
            });
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

            return Task.FromResult(clone[^1].Item);
        }
    }

    /// <inheritdoc/>
    public Task<long> DeleteItems(Expression<Func<TItem, bool>> filter)
    {
        var compiled = filter.Compile();

        long count = 0;

        /* Remove all matching items from dictionary. */
        lock (_onetimeInitializer.Data)
            foreach (var item in _onetimeInitializer.Data.ToArray())
                if (compiled(item.Value[^1].Item))
                {
                    count++;

                    _onetimeInitializer.Data.Remove(item.Key);
                }

        return Task.FromResult(count);
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
            return _onetimeInitializer.Data.Values.Select(list => CloneItem(list[^1].Item)).ToArray().AsQueryable();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<HistoryInfo>> GetHistory(string id)
    {
        /* Find in dictionary. */
        lock (_onetimeInitializer.Data)
        {
            if (!_onetimeInitializer.Data.TryGetValue(id, out var list))
                list = [];

            return Task.FromResult(list.Select(i => CloneItem(i).Version).Reverse());
        }
    }

    /// <inheritdoc/>
    public Task<TItem> GetHistoryItem(string id, long version)
    {
        /* Find in dictionary. */
        lock (_onetimeInitializer.Data)
        {
            if (!_onetimeInitializer.Data.TryGetValue(id, out var list))
                list = [];

            return Task.FromResult(CloneItem(list.Single(i => i.Version.ChangeCount == version).Item));
        }
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
                    var latest = i[^1].Item;
                    var oldValue = (string)getKey.Invoke(latest);

                    if (!caseSensitive) oldValue = oldValue?.ToLowerInvariant();

                    return oldValue == newValue;
                });

                if (match == null) return true;

                return match[^1].Item.Id == newItem.Id;
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
public class InMemoryHistoryCollectionFactory<TItem, TInitializer>(InMemoryHistoryCollection<TItem, TInitializer>.InitializerFactory factory, ICurrentUser user) : IHistoryCollectionFactory<TItem, TInitializer>
    where TItem : IDatabaseObject
    where TInitializer : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IHistoryCollection<TItem, TInitializer> Create(string uniqueName, string category)
    {
        var initializer = factory.GetOrAdd($"{category}:{uniqueName}");
        var collection = new InMemoryHistoryCollection<TItem, TInitializer>(initializer, user);

        initializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryHistoryCollectionFactory<TItem>(InMemoryHistoryCollection<TItem, NoopCollectionInitializer<TItem>>.InitializerFactory factory, ICurrentUser user) : InMemoryHistoryCollectionFactory<TItem, NoopCollectionInitializer<TItem>>(factory, user), IHistoryCollectionFactory<TItem>
    where TItem : IDatabaseObject
{
}
