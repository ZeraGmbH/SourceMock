using MeterTestSystemApi.Services;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a MT786/MT707/MT3000 connected to a serial port.
/// </summary>
public class MTSerialPortProbing : ISerialPortProbeExecutor
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