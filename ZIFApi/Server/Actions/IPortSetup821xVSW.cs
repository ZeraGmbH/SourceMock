namespace ZIFApi.Actions;

/// <summary>
/// Provides port setup from a JSON configuration file.
/// </summary>
public interface IPortSetup821xVSW
{
    /// <summary>
    /// Request the setups.
    /// </summary>
    Task<Dictionary<PortKey, byte[]>> PortSetups { get; }
}
