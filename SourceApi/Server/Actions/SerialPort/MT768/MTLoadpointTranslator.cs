using SerialPortProxy;

using SourceApi.Model;

namespace SourceApi.Actions.SerialPort.MT768;

/// <summary>
/// 
/// </summary>
public class MTLoadpointTranslator : LoadpointTranslator
{
    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public override SerialPortRequest[] ToSerialPortRequests(Loadpoint loadpoint)
    {
        var requests = new List<SerialPortRequest>();

        CreateFrequencyRequests("SFR", "SOKFR", loadpoint, requests);

        CreateVoltageRequests("SUP", "SOKUP", loadpoint, requests);

        CreateCurrentRequests("SIP", "SOKIP", loadpoint, requests);

        CreatePhaseRequests("SUI", "SOKUI", loadpoint, requests);

        return requests.ToArray();
    }
}
