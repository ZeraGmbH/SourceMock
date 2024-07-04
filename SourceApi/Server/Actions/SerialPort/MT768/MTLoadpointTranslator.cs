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
    public override SerialPortRequest[] ToSerialPortRequestsNGX(TargetLoadpointNGX loadpoint)
    {
        var loadpointDIN = ConvertFromIECtoDinNGX(loadpoint);
        var requests = new List<SerialPortRequest>();

        CreateFrequencyRequestsNGX("SFR", "SOKFR", loadpointDIN, requests);

        CreateVoltageRequestsNGX("SUP", "SOKUP", loadpointDIN, requests);

        CreateCurrentRequestsNGX("SIP", "SOKIP", loadpointDIN, requests);

        CreatePhaseRequestsNGX("SUI", "SOKUI", loadpointDIN, requests);

        return requests.ToArray();
    }
}
