using SerialPortProxy;

using SourceApi.Model;

namespace SourceApi.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public interface ILoadpointTranslator
{
    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public SerialPortRequest[] ToSerialPortRequests(TargetLoadpoint loadpoint);

    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public SerialPortRequest[] ToSerialPortRequestsNGX(TargetLoadpointNGX loadpoint);
}
