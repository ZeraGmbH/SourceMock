namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All protocol configurations - these are static and can not be
/// changed.
/// </summary>
public class ProtocolConfiguration
{
    /// <summary>
    /// Protocolls over TCP/IP.
    /// </summary>
    public IPProtocolConfigurations IP { get; set; } = new();
}