using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Run probe request against a serial port connection -
/// may be physical, USB connected or networt.
/// </summary>
public class ProbeSerialPort(IServiceProvider services, ISerialPortConnectionForProbing factory) : ProbeExecutor<SerialProbe>
{
    /// <inheritdoc/>
    protected override async Task<ProbeInfo> OnExecuteAsync(SerialProbe probe)
    {
        /* Create the algorithm. */
        var algorithm = services.GetRequiredKeyedService<ISerialPortProbeExecutor>(probe.Protocol);

        /* Create options and update. */
        var options = new SerialPortOptions { ReadTimeout = 2000, WriteTimeout = 2000 };

        algorithm.AdjustOptions(options);

        /* Create the connection. */
        using var connection = factory.Create(probe.DevicePath, options, algorithm.EnableReader);

        /* Create a new serial port connection accordung to the configuration. */
        return await algorithm.ExecuteAsync(probe, connection);
    }
}