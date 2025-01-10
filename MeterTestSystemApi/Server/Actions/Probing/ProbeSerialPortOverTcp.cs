using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;

namespace MeterTestSystemApi.Actions.Probing;

/// <summary>
/// Run probe request against a serial port connection 
/// using a pass-through TCP proxy
/// </summary>
public class ProbeSerialPortOverTcp(IServiceProvider services, ISerialPortConnectionForProbing factory) : ProbeExecutor<SerialProbeOverTcp>
{
    /// <inheritdoc/>
    protected override async Task<ProbeInfo> OnExecuteAsync(SerialProbeOverTcp probe)
    {
        /* Create the algorithm. */
        var algorithm = services.GetRequiredKeyedService<ISerialPortProbeExecutor>(probe.Protocol);

        /* Create the connection. */
        using var connection = factory.CreateNetwork(probe.Endpoint, 2000, algorithm.EnableReader);

        /* Create a new serial port connection accordung to the configuration. */
        return await algorithm.ExecuteAsync(connection);
    }
}