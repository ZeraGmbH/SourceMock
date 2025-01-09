using MeterTestSystemApi.Services;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Interface fo rprobing over a serial port.
/// </summary>
public interface ISerialPortProbeExecutor
{
    /// <summary>
    /// Set to enable the serial port connection background reader.
    /// </summary>
    bool EnableReader { get; }

    /// <summary>
    /// Adjust the serial port options if necessary.
    /// </summary>
    /// <param name="options">Options to use.</param>
    void AdjustOptions(SerialPortOptions options);

    /// <summary>
    /// Run a single probing algorithm.
    /// </summary>
    /// <param name="probe">Probe configuration.</param>
    /// <param name="connection">Connectiojn to use.</param>
    /// <returns>Error message or empty.</returns>
    Task<ProbeInfo> ExecuteAsync(SerialProbe probe, ISerialPortConnection connection);
}