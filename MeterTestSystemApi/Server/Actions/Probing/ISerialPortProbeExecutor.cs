using MeterTestSystemApi.Services;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Interface fo rprobing over a serial port.
/// </summary>
public interface ISerialPortProbeExecutor
{
    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Probe configuration.</param>
    /// <returns>Error message or empty.</returns>
    Task<string> ExecuteAsync(SerialProbe probe);
}