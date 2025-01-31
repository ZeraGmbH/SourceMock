using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Storage interface for the meter test system configuration.
/// </summary>
public interface IMeterTestSystemConfigurationStore
{
    /// <summary>
    /// Read the current configuration.
    /// </summary>
    /// <returns>Current configuration.</returns>
    public Task<MeterTestSystemConfiguration> ReadAsync();

    /// <summary>
    /// Write a new configuration.
    /// </summary>
    /// <param name="config">New configuration.</param>
    /// <returns>New configuration as stored in the database.</returns>
    public Task<MeterTestSystemConfiguration> WriteAsync(MeterTestSystemConfiguration config);

    /// <summary>
    /// Get the history.
    /// </summary>
    /// <returns>History informnation of the item sorted by version descending - i.e. newest first.</returns>
    Task<IEnumerable<HistoryInfo>> GetHistoryAsync();

    /// <summary>
    /// Retrieve a specific version.
    /// </summary>
    /// <param name="version">The version number to look up - starting with 1.</param>
    /// <returns>The item in the indicated version</returns>
    Task<MeterTestSystemConfiguration> GetHistoryItemAsync(long version);

    /// <summary>
    /// Startt probing mode.
    /// </summary>
    public Task StartProbingAsync();

    /// <summary>
    /// Reset probing mode.
    /// </summary>
    public Task ResetProbingAsync();
}