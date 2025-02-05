using SerialPortProxy;

namespace BurdenApi.Models;

/// <summary>
/// 
/// </summary>
public class BurdenConfiguration
{
    /// <summary>
    /// Serial port to connect to the burden (ESCB or ESVB).
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }

    /// <summary>
    /// Set to simulate hardware access.
    /// </summary>
    public bool SimulateHardware { get; set; }
}