using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a MT786/MT707/MT3000 connected to a serial port.
/// </summary>
public class MTSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public Task<string> ExecuteAsync(SerialProbe probe)
    {
        throw new NotImplementedException();
    }
}