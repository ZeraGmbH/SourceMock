using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Run probe request against a serial port connection -
/// may be physical, USB connected or networt.
/// </summary>
public class ProbeSerialPort(IServiceProvider services) : IProbeExecutor<SerialProbe>
{
    /// <inheritdoc/>
    public Task<string> ExecuteAsync(SerialProbe probe)
    {
        return services.GetRequiredKeyedService<ISerialPortProbeExecutor>(probe.Protocol).ExecuteAsync(probe);
    }
}