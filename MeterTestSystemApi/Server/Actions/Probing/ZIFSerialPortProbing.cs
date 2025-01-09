using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a ZIF socket connected to a serial port.
/// </summary>
public class ZIFSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public Task<string> ExecuteAsync(SerialProbe probe)
    {
        throw new NotImplementedException();
    }
}