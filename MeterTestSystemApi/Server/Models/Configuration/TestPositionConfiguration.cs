using ErrorCalculatorApi.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Configuration of a single test position.
/// </summary>
public class TestPositionConfiguration
{
    /// <summary>
    /// Set if the position is in use.
    /// </summary>
    public required bool Enabled { get; set; }

    /// <summary>
    /// Protocol version to use when communicating with the MAD server.
    /// If not set the error calculator can not be used on this
    /// position.
    /// </summary>
    public ErrorCalculatorProtocols? MadProtocol { get; set; }

    /// <summary>
    /// STM server connection to use.
    /// </summary>
    public ServerTypes? STMServer { get; set; }

    /// <summary>
    /// Set to enable use of the update server.
    /// </summary>
    public bool EnableUpdateServer { get; set; }

    /// <summary>
    /// Set to enable direct connection to the device under test.
    /// </summary>
    public bool EnableDirectDutConnection { get; set; }

    /// <summary>
    /// Set to use the UART interface.
    /// </summary>
    public bool EnableUART { get; set; }

    /// <summary>
    /// Set to enable the old UART interface.
    /// </summary>
    public bool EnableLegacyUART { get; set; }

    /// <summary>
    /// Set to enable the old OA interface - STM6000 only.
    /// </summary>
    public bool EnableLegacyOA { get; set; }

    /// <summary>
    /// Set to enable direct access to the COM server.
    /// </summary>
    public bool EnableCOMServer { get; set; }

    /// <summary>
    /// Set to use SIM server 1.
    /// </summary>
    public bool EnableSIMServer1 { get; set; }

    /// <summary>
    /// Set to access the backend gateway.
    /// </summary>
    public bool EnableBackendGateway { get; set; }

    /// <summary>
    /// Make sure that the position parameter is valid.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Position is not in the indicated range.</exception>
    public static void AssertPosition(int position)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, 80);
    }
}
