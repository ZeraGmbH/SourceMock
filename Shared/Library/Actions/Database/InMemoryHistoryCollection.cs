using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using DeviceApiSharedLibrary.Models;
using DeviceApiLib.Actions.Database;

namespace DeviceApiSharedLibrary.Actions.Database;

/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
public sealed class InMemoryHistoryCollection<TItem> : IHistoryCollection<TItem> where TItem : DatabaseObject
{
    private readonly ILogger<InMemoryHistoryCollection<TItem>> _logger;

    private readonly Dictionary<string, List<HistoryItem<TItem>>> _data = new();

    /// <summary>
    /// Initializes a new collection.
    /// </summary>
    /// <param name="logger">Logging instance to use.</param>
    public InMemoryHistoryCollection(ILogger<InMemoryHistoryCollection<TItem>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Clone item using the standard MongoDb serialisation mechanisms. 
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <typeparam name="T">Type of the item to clone.</typeparam>
    /// <returns>Clone of the item.</returns>
    public static T CloneItem<T>(T item)
    {
        /* Make us behave just like real implementations will do. */
        using var writer = new BsonDocumentWriter(new BsonDocument());

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
            Item = InMemoryHistoryCollection<TItem>.CloneItem(item),
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
        lock (_data)
            if (!_data.TryAdd(clone.Item.Id, new List<HistoryItem<TItem>> { clone }))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(InMemoryHistoryCollection<TItem>.CloneItem(clone.Item));
    }

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item, string user)
    {
        var now = DateTime.Now;

        /* Always create a fully detached clone. */
        var clone = InMemoryHistoryCollection<TItem>.CloneItem(item);

        /* Replace in dictionary. */
        lock (_data)
        {
            if (!_data.ContainsKey(clone.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            var list = _data[clone.Id];
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
        return Task.FromResult(InMemoryHistoryCollection<TItem>.CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> DeleteItem(string id, string user)
    {
        /* Remove from dictionary. */
        lock (_data)
        {
            if (!_data.TryGetValue(id, out var clone))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(id)));

            _data.Remove(id);

            return Task.FromResult(clone[^1].Item);
        }
    }

    /// <inheritdoc/>
    public Task<IEnumerable<HistoryItem<TItem>>> GetHistory(string id)
    {
        /* Find in dictionary. */
        lock (_data)
        {
            if (!_data.TryGetValue(id, out var list))
                list = new();

            return Task.FromResult(list.Select(i => InMemoryHistoryCollection<TItem>.CloneItem(i)).Reverse());
        }
    }

    /// <inheritdoc/>
    public Task<long> RemoveAll()
    {
        lock (_data)
            try
            {
                return Task.FromResult<long>(_data.Count);
            }
            finally
            {
                _data.Clear();
            }
    }

    /// <inheritdoc/>
    public IQueryable<TItem> CreateQueryable()
    {
        lock (_data)
            return _data.Values.Select(list => CloneItem(list[^1].Item)).ToArray().AsQueryable();
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class InMemoryHistoryCollectionFactory<TItem> : IHistoryCollectionFactory<TItem> where TItem : DatabaseObject
{
    private readonly ILogger<InMemoryHistoryCollection<TItem>> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public InMemoryHistoryCollectionFactory(ILogger<InMemoryHistoryCollection<TItem>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public IHistoryCollection<TItem> Create(string uniqueName) => new InMemoryHistoryCollection<TItem>(_logger);
}

