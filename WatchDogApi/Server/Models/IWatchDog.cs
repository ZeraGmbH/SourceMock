namespace WatchDogApi.Models;

/// <summary>
/// Represents a single watchdog protocol.
/// </summary>
public interface IWatchDog
{
    /// <summary>
    /// set a new config to the watchdog
    /// </summary>
    /// <param name="config">config to set</param>
    public void SetConfig(WatchDogConfiguration config);
}