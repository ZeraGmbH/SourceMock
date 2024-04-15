using System.Linq.Expressions;
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
sealed class InMemoryHistoryCollection<TItem, TSingleton>(InMemoryHistoryCollection<TItem, TSingleton>.Initializer singleton) : IHistoryCollection<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    /// <summary>
    /// 
    /// </summary>
    public class Initializer(TSingleton singleton) : CollectionInitializer<TItem>
    {
        public readonly TSingleton Singleton = singleton;

        public Dictionary<string, List<HistoryItem<TItem>>> Data = null!;

        public event Func<TItem, bool>? IndexCheck = null;

        public bool? CheckIndex(TItem item) => IndexCheck?.Invoke(item);

        protected override Task OnInitialize(IObjectCollection<TItem> collection)
        {
            Data = [];

            return Singleton.Initialize(collection);
        }
    }

    public TSingleton Common => singleton.Singleton;

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
    public Task<TItem> AddItem(TItem item, string user)
    {
        /* Create all fields used for historisation. */
        var now = DateTime.Now;

        /* Always create a fully detached clone. */
        var clone = new HistoryItem<TItem>
        {
            Item = CloneItem(item),
            Version = new HistoryInfo
            {
                ChangeCount = 1,
                CreatedAt = now,
                CreatedBy = user,
                ModifiedAt = now,
                ModifiedBy = user
            }
        };

        /* Add to dictionary. */
        lock (singleton.Data)
        {
            if (singleton.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!singleton.Data.TryAdd(clone.Item.Id, [clone]))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone.Item));
    }

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item, string user)
    {
        var now = DateTime.Now;

        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Replace in dictionary. */
        lock (singleton.Data)
        {
            if (singleton.CheckIndex(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!singleton.Data.ContainsKey(clone.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            var list = singleton.Data[clone.Id];
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
                    ModifiedBy = user
                }
            });
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, string user, bool silent = false)
    {
        /* Remove from dictionary. */
        lock (singleton.Data)
        {
            if (!singleton.Data.TryGetValue(id, out var clone))
                return silent ? Task.FromResult(default(TItem)!) : Task.FromException<TItem>(new ArgumentException("item not found", nameof(id)));

            singleton.Data.Remove(id);

            return Task.FromResult(clone[^1].Item);
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
            return singleton.Data.Values.Select(list => CloneItem(list[^1].Item)).ToArray().AsQueryable();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<HistoryInfo>> GetHistory(string id)
    {
        /* Find in dictionary. */
        lock (singleton.Data)
        {
            if (!singleton.Data.TryGetValue(id, out var list))
                list = [];

            return Task.FromResult(list.Select(i => CloneItem(i).Version).Reverse());
        }
    }

    /// <inheritdoc/>
    public Task<TItem> GetHistoryItem(string id, long version)
    {
        /* Find in dictionary. */
        lock (singleton.Data)
        {
            if (!singleton.Data.TryGetValue(id, out var list))
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

            singleton.IndexCheck += (newItem) =>
            {
                var newValue = (string)getKey.Invoke(newItem);

                if (!caseSensitive) newValue = newValue?.ToLowerInvariant();

                var match = singleton.Data.Values.SingleOrDefault(i =>
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
/// <typeparam name="TSingleton"></typeparam>
public class InMemoryHistoryCollectionFactory<TItem, TSingleton>(TSingleton singleton) : IHistoryCollectionFactory<TItem, TSingleton>
    where TItem : IDatabaseObject
    where TSingleton : ICollectionInitializer<TItem>
{
    /// <inheritdoc/>
    public IHistoryCollection<TItem, TSingleton> Create(string uniqueName, string category)
    {
        var initializer = new InMemoryHistoryCollection<TItem, TSingleton>.Initializer(singleton);
        var collection = new InMemoryHistoryCollection<TItem, TSingleton>(initializer);

        initializer.Initialize(collection).Wait();

        return collection;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryHistoryCollectionFactory<TItem>() : InMemoryHistoryCollectionFactory<TItem, NoopInitializer<TItem>>(new()), IHistoryCollectionFactory<TItem> where TItem : IDatabaseObject
{
}
