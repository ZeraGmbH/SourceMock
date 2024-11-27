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
    /// <param name="loadpoint">Some already validated loadpoint in IEC form.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public override SerialPortRequest[] ToSerialPortRequests(TargetLoadpoint loadpoint)
    {
        var DINloadpoint = ConvertFromIECtoDin(loadpoint);
        var requests = new List<SerialPortRequest>
        {
            // Switch off DC subtraction
            SerialPortRequest.Create("DS1", "OKDS")
        };

        CreateFrequencyRequests("FR", "OKFR", DINloadpoint, requests);

        CreateVoltageRequests("UP", "OKUP", DINloadpoint, requests);

        CreateCurrentRequests("IP", "OKIP", DINloadpoint, requests);

        CreatePhaseRequests("UI", "OKUI", DINloadpoint, requests);

        return requests.ToArray();
    }
}
