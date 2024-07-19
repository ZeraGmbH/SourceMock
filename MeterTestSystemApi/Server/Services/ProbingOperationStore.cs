using MeterTestSystemApi.Models.Configuration;
using ZERA.WebSam.Shared.Actions.Database;
using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Services;

/// <summary>
/// 
/// </summary>
public class ProbingOperationStore(IHistoryCollectionFactory<ProbingOperation> factory) : IProbingOperationStore
{
    private readonly IHistoryCollection<ProbingOperation> _collection = factory.Create("sam-configuration-probings", DatabaseCategories.MeterTestSystem);

    /// <summary>
    /// Underlying collection is made available for unit tests.
    /// </summary>
    public IHistoryCollection<ProbingOperation> Collection => _collection;

    /// <inheritdoc/>
    public Task<ProbingOperation> Add(ProbingOperation operation) => _collection.AddItem(operation);

    /// <inheritdoc/>
    public Task<ProbingOperation?> Get(string id) => _collection.GetItem(id);

    /// <inheritdoc/>
    public IQueryable<ProbingOperation> Query() => _collection.CreateQueryable();

    /// <inheritdoc/>
    public Task<ProbingOperation> Update(ProbingOperation operation) => _collection.UpdateItem(operation);
}