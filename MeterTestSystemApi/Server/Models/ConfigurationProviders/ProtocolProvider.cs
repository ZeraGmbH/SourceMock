namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// All protocol configurations - these are static and can not be
/// changed.
/// </summary>
public class ProtocolProvider
{
    /// <summary>
    /// Protocolls over TCP/IP.
    /// </summary>
    public IPProtocolProvider IP { get; set; } = new();
}