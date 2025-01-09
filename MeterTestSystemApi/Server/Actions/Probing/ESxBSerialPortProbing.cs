using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Probe for a burden connected to a serial port.
/// </summary>
public class ESxBSerialPortProbing : ISerialPortProbeExecutor
{
    /// <inheritdoc/>
    public Task<string> ExecuteAsync(SerialProbe probe)
    {
        throw new NotImplementedException();
    }
}