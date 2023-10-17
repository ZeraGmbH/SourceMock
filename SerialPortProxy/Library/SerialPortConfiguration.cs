namespace SerialPortProxy;

/// <summary>
/// Configuration for a serial port service.
/// </summary>
public class SerialPortConfiguration
{
    /// <summary>
    /// Name of the serial port to use, e.g. COM3 on Windows or
    /// /dev/ttyUSB0 unter Linux. Alternativly the type of a 
    /// mock class.
    /// </summary>
    public string PortNameOrMockType { get; set; } = null!;

    /// <summary>
    /// If set PortNameOrMockType is the name of a .NET type 
    /// mocking a physical connection - else the name of a 
    /// serial port is used.
    /// </summary>
    public bool UseMockType { get; set; }
}