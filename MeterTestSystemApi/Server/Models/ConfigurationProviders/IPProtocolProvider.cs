using System.Net;
using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationProviders;

/// <summary>
/// All endpoint construction algorithms.
/// </summary>
public static class IPProtocolProvider
{
    /// <summary>
    /// For a given test position calculate the related IP for an STM 4000.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <returns>IP of the corresponding STM 4000.</returns>
    private static byte GetIpForStm4000(int position) => (byte)(181 + (position - 1) / 10);

    /// <summary>
    /// For a given test position calculate the related IP for an STM 6000.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <returns>IP of the corresponding STM 6000.</returns>
    private static byte GetIpForStm6000(int position) => (byte)(position + 100);

    /// <summary>
    /// Calculate the port number for a test position connected to a STM 4000.
    /// </summary>
    /// <param name="firstPort">Port for test position 1.</param>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <returns>The requested port.</returns>
    private static ushort GetPortForStm4000(ushort firstPort, int position) => (ushort)(firstPort + (position - 1) % 10 * 100);

    /// <summary>
    /// Calculate the endpoint for the MAD server of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetMadEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM4000 => new IPEndPointProvider { IP = GetIpForStm4000(position), Port = GetPortForStm4000(14007, position) },
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14207 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the updated server of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetUpdateEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM4000 => new IPEndPointProvider { IP = GetIpForStm4000(position), Port = 14196 },
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14196 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the direct connection to a device
    /// unter test of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetDirectDutConnectionEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM4000 => new IPEndPointProvider { IP = GetIpForStm4000(position), Port = GetPortForStm4000(14002, position) },
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14202 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the UART interface of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetUARTEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM4000 => new IPEndPointProvider { IP = GetIpForStm4000(position), Port = GetPortForStm4000(14009, position) },
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14209 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the old OA interface of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetObjectAccessEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14204 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the STM6000 COM server of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetCOMServerEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14201 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the STM6000 SIM server 1 of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetSIMServer1Endpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14208 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the endpoint for the gateway between WebSam and meter of any test position.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <param name="type">Type of the STM providing the connection.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetBackendGatewayEndpoint(int position, ServerTypes type)
    {
        TestPositionConfiguration.AssertPosition(position);

        return type switch
        {
            ServerTypes.STM6000 => new IPEndPointProvider { IP = GetIpForStm6000(position), Port = 14198 },
            _ => throw new ArgumentException("unsupported server type", nameof(type)),
        };
    }

    /// <summary>
    /// Calculate the CR2020 endpoint to control the MP2020.
    /// </summary>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint Get2020ControlEndpoint() => new IPEndPointProvider { IP = 100, Port = 14200 };
}