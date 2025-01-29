using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// 
/// </summary>
public interface IProbingOperationStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    Task<ProbingOperation> AddAsync(ProbingOperation operation);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="operation"></param>
    /// <returns></returns>
    Task<ProbingOperation> UpdateAsync(ProbingOperation operation);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IQueryable<ProbingOperation> Query();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ProbingOperation?> GetAsync(string id);
}