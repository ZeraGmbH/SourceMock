using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ErrorCalculatorApi.Models;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Configuration of a single test position.
/// </summary>
public class TestPositionConfiguration
{
    /// <summary>
    /// Maximum number of test positions - before getting IP conflicts.
    /// </summary>
    public const int MaxPosition = 80;

    /// <summary>
    /// Set if the position is in use.
    /// </summary>
    [Required, NotNull]
    public bool Enabled { get; set; }

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
    [Required, NotNull]
    public bool EnableUpdateServer { get; set; }

    /// <summary>
    /// Set to enable direct connection to the device under test.
    /// </summary>
    [Required, NotNull]
    public bool EnableDirectDutConnection { get; set; }

    /// <summary>
    /// Set to use the UART interface.
    /// </summary>
    [Required, NotNull]
    public bool EnableUART { get; set; }

    /// <summary>
    /// Set to use the MAD interface.
    /// </summary>
    [Required, NotNull]
    public bool EnableMAD { get; set; }

    /// <summary>
    /// Set to enable the object access interface - STM6000 only.
    /// </summary>
    [Required, NotNull]
    public bool EnableObjectAccess { get; set; }

    /// <summary>
    /// Set to enable direct access to the COM server.
    /// </summary>
    [Required, NotNull]
    public bool EnableCOMServer { get; set; }

    /// <summary>
    /// Set to use SIM server 1.
    /// </summary>
    [Required, NotNull]
    public bool EnableSIMServer1 { get; set; }

    /// <summary>
    /// Set to access the backend gateway.
    /// </summary>
    [Required, NotNull]
    public bool EnableBackendGateway { get; set; }

    /// <summary>
    /// Make sure that the position parameter is valid.
    /// </summary>
    /// <param name="position">Position index between 1 and 80 - both inclusive.</param>
    /// <exception cref="ArgumentOutOfRangeException">Position is not in the indicated range.</exception>
    public static void AssertPosition(uint position)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, (uint)1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(position, (uint)MaxPosition);
    }
}
