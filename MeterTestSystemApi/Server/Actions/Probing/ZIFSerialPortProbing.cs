using MeterTestSystemApi.Services;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a ZIF socket connected to a serial port.
/// </summary>
public class ZIFSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public bool EnableReader => false;

    /// <inheritdoc/>
    public void AdjustOptions(SerialPortOptions options) { }

    /// <inheritdoc/>
    public Task<ProbeInfo> ExecuteAsync(SerialProbe probe, ISerialPortConnection connection)
    {
        throw new NotImplementedException();
    }
}