using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a FG30x connected to a serial port.
/// </summary>
public class FGSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public Task<string> ExecuteAsync(SerialProbe probe)
    {
        throw new NotImplementedException();
    }
}