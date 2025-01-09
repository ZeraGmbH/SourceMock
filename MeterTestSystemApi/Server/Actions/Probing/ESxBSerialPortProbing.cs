using MeterTestSystemApi.Services;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a burden connected to a serial port.
/// </summary>
public class ESxBSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public bool EnableReader => true;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    /// <inheritdoc/>
    public Task<ProbeInfo> ExecuteAsync(SerialProbe probe, ISerialPortConnection connection)
    {
        throw new NotImplementedException();
    }
}