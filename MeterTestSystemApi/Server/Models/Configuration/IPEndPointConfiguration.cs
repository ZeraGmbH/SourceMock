using System.Net;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Describe a single IP endpoint in the test network.
/// </summary>
public class IPEndPointConfiguration
{
    /// <summary>
    /// The predefined subnet for all test components.
    /// </summary>
    public const string TestNetPattern = "192.168.32.{IP}";

    /// <summary>
    /// The last byte of the IP address.
    /// </summary>
    public required byte IP { get; set; }

    /// <summary>
    /// The related IP port - protocol used on the endpoint may vary.
    /// </summary>
    public required ushort Port { get; set; }

    /// <summary>
    /// Construct the full endpoint address from the properties.
    /// </summary>
    public IPEndPoint Address => new(IPAddress.Parse(TestNetPattern.Replace("{IP}", IP.ToString())), Port);
}

