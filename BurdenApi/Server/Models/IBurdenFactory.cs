namespace BurdenApi.Models;

/// <summary>
/// 
/// </summary>
public interface IBurdenFactory : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    void Initialize(BurdenConfiguration configuration);

    /// <summary>
    /// 
    /// </summary>
    IBurden Burden { get; }
}