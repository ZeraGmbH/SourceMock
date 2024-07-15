using System.Net;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// 
/// </summary>
public class IPProtocolConfigurations
{
    /// <summary>
    /// Calculate the MAD endpoint for the MAD server of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 100 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.d</returns>
    public static IPEndPointConfiguration GetMadEndpoint(int position, STMTypes type)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, 100);

        return type switch
        {
            STMTypes.STM4000 => new() { IP = (byte)(181 + (position - 1) / 10), Port = (ushort)(14007 + (position - 1) % 10 * 100) },
            STMTypes.STM6000 => new() { IP = (byte)(position + 100), Port = 14207 },
            _ => throw new ArgumentException("unsupported MAD server type", nameof(type)),
        };
    }
}