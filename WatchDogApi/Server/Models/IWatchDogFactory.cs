namespace WatchDogApi.Models;

/// <summary>
/// Initializer and provider of THE watchdog device.
/// </summary>
public interface IWatchDogFactory : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    void Initialize(WatchDogConfiguration config);

    /// <summary>
    /// 
    /// </summary>
    IWatchDog WatchDog { get; }
}