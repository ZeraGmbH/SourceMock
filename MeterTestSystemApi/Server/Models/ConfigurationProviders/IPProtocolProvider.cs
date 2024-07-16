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

    /// <summary>
    /// Calculate the endpoint of any DC component.
    /// </summary>
    /// <param name="component">DC component of interest.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetDCComponentEndpoint(DCComponents component)
        => component switch
        {
            DCComponents.CurrentSCG06 => new IPEndPointProvider { IP = 83, Port = 10001 },
            DCComponents.CurrentSCG1000 => new IPEndPointProvider { IP = 84, Port = 10001 },
            DCComponents.CurrentSCG750 => new IPEndPointProvider { IP = 82, Port = 10001 },
            DCComponents.CurrentSCG8 => new IPEndPointProvider { IP = 80, Port = 10001 },
            DCComponents.CurrentSCG80 => new IPEndPointProvider { IP = 81, Port = 10001 },
            DCComponents.FGControl => new IPEndPointProvider { IP = 91, Port = 13000 },
            DCComponents.SPS => new IPEndPointProvider { IP = 200, Port = 0 },
            DCComponents.VoltageSVG1200 => new IPEndPointProvider { IP = 85, Port = 10001 },
            DCComponents.VoltageSVG150 => new IPEndPointProvider { IP = 89, Port = 10001 },
            _ => throw new ArgumentException("unsupported DC component or combination of components", nameof(component)),
        };

    /// <summary>
    /// Calculate the endpoint of any transformer component.
    /// </summary>
    /// <param name="component">Transformer component of interest.</param>
    /// <returns>The configuration for the endpoint.</returns>
    public static IPEndPoint GetTransformerComponentEndpoint(TransformerComponents component)
        => component switch
        {
            TransformerComponents.CurrentWM3000or1000 => new IPEndPointProvider { IP = 211, Port = 6315 },
            TransformerComponents.SPS => new IPEndPointProvider { IP = 200, Port = 0 },
            TransformerComponents.STR260Phase1 => new IPEndPointProvider { IP = 201, Port = 0 },
            TransformerComponents.STR260Phase2 => new IPEndPointProvider { IP = 202, Port = 0 },
            TransformerComponents.STR260Phase3 => new IPEndPointProvider { IP = 203, Port = 0 },
            TransformerComponents.VoltageWM3000or1000 => new IPEndPointProvider { IP = 221, Port = 6315 },
            _ => throw new ArgumentException("unsupported transformer component or combination of components", nameof(component)),
        };
}