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

        /* Create options. */
        var options = new SerialPortOptions { ReadTimeout = 2000, WriteTimeout = 2000 };

        /* Check for overrides. */
        var devOptions = probe.Device.Options;

        if (devOptions != null)
        {
            if (devOptions.BaudRate != null) options.BaudRate = devOptions.BaudRate;
            if (devOptions.DataBits != null) options.DataBits = devOptions.DataBits;
            if (devOptions.NewLine != null) options.NewLine = devOptions.NewLine;
            if (devOptions.Parity != null) options.Parity = devOptions.Parity;
            if (devOptions.ReadTimeout != null) options.ReadTimeout = devOptions.ReadTimeout;
            if (devOptions.StopBits != null) options.StopBits = devOptions.StopBits;
            if (devOptions.WriteTimeout != null) options.WriteTimeout = devOptions.WriteTimeout;
        }

        /* Let algorithm to make the final adjustments. */
        algorithm.AdjustOptions(options);

        /* Create the connection. */
        using var connection = factory.CreatePhysical(probe.DevicePath, options, algorithm.EnableReader);

        /* Create a new serial port connection accordung to the configuration. */
        return await algorithm.ExecuteAsync(connection);
    }
}