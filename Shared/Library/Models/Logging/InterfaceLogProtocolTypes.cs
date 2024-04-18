namespace SharedLibrary.Models.Logging;

/// <summary>
/// Physical type of the communication.
/// </summary>
public enum InterfaceLogProtocolTypes
{
    /// <summary>
    /// Serial port.
    /// </summary>
    Com = 0,

    /// <summary>
    /// Network with TCP stream connection.
    /// </summary>
    Tcp = 1,

    /// <summary>
    /// Network with UDP packet connection.
    /// </summary>
    Udp = 2
}