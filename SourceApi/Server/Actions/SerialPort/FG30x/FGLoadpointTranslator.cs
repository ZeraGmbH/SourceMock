using SerialPortProxy;

using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.FG30x;

/// <summary>
/// 
/// </summary>
public class FGLoadpointTranslator : LoadpointTranslator
{
    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public override SerialPortRequest[] ToSerialPortRequests(Loadpoint loadpoint)
    {
        var requests = new List<SerialPortRequest>();

        CreateFrequencyRequests("FR", "OKFR", loadpoint, requests);

        CreateVoltageRequests("UP", "OKUP", loadpoint, requests);

        CreateCurrentRequests("IP", "OKIP", loadpoint, requests);

        CreatePhaseRequests("UI", "OKUI", loadpoint, requests);

        return requests.ToArray();
    }
}
