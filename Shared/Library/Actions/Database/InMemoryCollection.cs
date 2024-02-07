
using System.Linq.Expressions;
using System.Security.Permissions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SharedLibrary.Models;

namespace SharedLibrary.Actions.Database;

/// <summary>
/// In memory collection.
/// </summary>
/// <typeparam name="TItem">Type of the related document.</typeparam>
/// <remarks>
/// Initializes a new collection.
/// </remarks>
public sealed class InMemoryCollection<TItem> : IObjectCollection<TItem> where TItem : IDatabaseObject
{

    private readonly Dictionary<string, TItem> _data = [];

    private event Func<TItem, bool>? _indexCheck = null;

    /// <summary>
    /// Clone item using the standard MongoDb serialisation mechanisms. 
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
    public Task<TItem> AddItem(TItem item, string user)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Add to dictionary. */
        lock (_data)
        {
            if (_indexCheck?.Invoke(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_data.TryAdd(clone.Id, clone))
                return Task.FromException<TItem>(new ArgumentException("duplicate item", nameof(item)));
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
    }

    /// <inheritdoc/>
    public Task<TItem> UpdateItem(TItem item, string user)
    {
        /* Always create a fully detached clone. */
        var clone = CloneItem(item);

        /* Replace in dictionary. */
        lock (_data)
        {
            if (_indexCheck?.Invoke(item) == false)
                return Task.FromException<TItem>(new TaskCanceledException("index"));

            if (!_data.ContainsKey(item.Id))
                return Task.FromException<TItem>(new ArgumentException("item not found", nameof(item)));

            _data[clone.Id] = clone;
        }

        /* Report a second clone to make sure no one messes with our internal structure. */
        return Task.FromResult(CloneItem(clone));
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

    /// <inheritdoc/>
    public Task<string> CreateIndex(string name, Expression<Func<TItem, object>> keyAccessor, bool ascending = true, bool unique = true, bool caseSensitive = true)
    {
        if (unique)
        {
            var getKey = keyAccessor.Compile();

            _indexCheck += (newItem) =>
            {
                var newValue = (string)getKey.Invoke(newItem);

                if (!caseSensitive) newValue = newValue?.ToLowerInvariant();

                var match = _data.Values.SingleOrDefault(i =>
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
public class InMemoryCollectionFactory<TItem> : IObjectCollectionFactory<TItem> where TItem : IDatabaseObject
{
    /// <inheritdoc/>
    public IObjectCollection<TItem> Create(string uniqueName) => new InMemoryCollection<TItem>();
}

