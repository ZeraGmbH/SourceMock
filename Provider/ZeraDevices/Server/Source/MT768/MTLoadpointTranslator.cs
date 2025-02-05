using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Source;

namespace ZeraDevices.Source.MT768;

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
    public override SerialPortRequest[] ToSerialPortRequests(TargetLoadpoint loadpoint)
    {
        var loadpointDIN = ConvertFromIECtoDin(loadpoint);
        var requests = new List<SerialPortRequest>();

        CreateFrequencyRequests("SFR", "SOKFR", loadpointDIN, requests);

        CreateVoltageRequests("SUP", "SOKUP", loadpointDIN, requests);

        CreateCurrentRequests("SIP", "SOKIP", loadpointDIN, requests);

        CreatePhaseRequests("SUI", "SOKUI", loadpointDIN, requests);

        return requests.ToArray();
    }
}
