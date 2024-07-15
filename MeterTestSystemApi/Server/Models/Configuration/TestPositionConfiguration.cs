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
    /// </summary>
    public ErrorCalculatorProtocols? MadProtocol { get; set; }

    /// <summary>
    /// MAD server connection to use.
    /// </summary>
    public MadServerTypes? MadServer { get; set; }
}
