using System.Net;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// Describe a single IP endpoint in the test network.
/// </summary>
public class IPEndPointProvider
{
    /// <summary>
    /// The predefined subnet for all test components.
    /// </summary>
    private const string TestNetPattern = "192.168.32.{IP}";

    /// <summary>
    /// Create a full IP address from the last byte.
    /// </summary>
    /// <param name="ip">Last byte of the IP address.</param>
    /// <returns>Full address.</returns>
    public static IPAddress MakeAddress(byte ip) => IPAddress.Parse(TestNetPattern.Replace("{IP}", ip.ToString()));

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
    public static implicit operator IPEndPoint(IPEndPointProvider endPoint) => new(MakeAddress(endPoint.IP), endPoint.Port);
}

