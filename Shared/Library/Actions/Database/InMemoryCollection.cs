
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using DeviceApiSharedLibrary.Models;

namespace DeviceApiSharedLibrary.Actions.Database;

/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
public abstract class InMemoryCollection<TItem> : IObjectCollection<TItem> where TItem : DatabaseObject
{
    private readonly ILogger<InMemoryCollection<TItem>> _logger;

    private readonly Dictionary<string, TItem> _data = new();

    /// <summary>
    /// Initializes a new collection.
    /// </summary>
    /// <param name="logger">Logging instance to use.</param>
    public InMemoryCollection(ILogger<InMemoryCollection<TItem>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Clone item using the standard MongoDb serialisation mechanisms. 
    /// </summary>
    /// <param name="item">Some item.</param>
    /// <returns>Clone of the item.</returns>
    private TItem CloneItem(TItem item)
    {
        /* Make us behave just like real implementations will do. */
        using var writer = new BsonDocumentWriter(new BsonDocument());

        BsonSerializer.Serialize(writer, item);

        return BsonSerializer.Deserialize<TItem>(writer.Document);
    }

    /// <inheritdoc/>
    public virtual Task<TItem> AddItem(TItem item, string user)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Add to dictionary. */
        lock (_data)
            if (!_data.TryAdd(clone.Id, clone))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public virtual Task<TItem> UpdateItem(TItem item, string user)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Replace in dictionary. */
        lock (_data)
        {
            if (!_data.ContainsKey(item.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            _data[clone.Id] = clone;
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public virtual Task<TItem> DeleteItem(string id, string user)
    {
        /* Remove from dictionary. */
        lock (_data)
        {
            if (!_data.TryGetValue(id, out var clone))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(id)));

            _data.Remove(id);

            return Task.FromResult(clone);
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
            return _data.Values.Select(CloneItem).ToArray().AsQueryable();
    }
}

