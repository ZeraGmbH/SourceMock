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
    public STMTypes? STMServer { get; set; }

    /// <summary>
    /// Set to enable use of the update server.
    /// </summary>
    public bool EnableUpdateServer { get; set; }
}
