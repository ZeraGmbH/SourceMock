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
    /// Startt probing mode.
    /// </summary>
    public Task StartProbingAsync();

    /// <summary>
    /// Reset probing mode.
    /// </summary>
    public Task ResetProbingAsync();
}