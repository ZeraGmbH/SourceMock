using SerialPortProxy;

using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public class FGLoadpointTranslator : ILoadpointTranslator
{
    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public SerialPortRequest[] ToSerialPortRequests(Loadpoint loadpoint)
    {
        throw new NotImplementedException("ToSerialPortRequests");
    }
}
