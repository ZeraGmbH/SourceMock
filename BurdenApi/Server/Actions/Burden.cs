using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Actions;

/// <summary>
/// Implementation of the burden communication interface.
/// </summary>
/// <param name="device">Serial port connection to the hardware.</param>
public class Burden([FromKeyedServices("Burden")] ISerialPortConnection device) : IBurden
{
    /// <inheritdoc/>
    public async Task<BurdenVersion> GetVersionAsync(IInterfaceLogger log)
    {
        // Request version from device.
        var version = await device
            .CreateExecutor(InterfaceLogSourceTypes.Burden)
            .ExecuteAsync(log, SerialPortRequest.Create("AV", "AVACK"))[0];

        if (version.Length < 3) throw new InvalidOperationException($"to few resonse lines");

        // Create reply.
        return new() { Version = version[^3], Supplement = version[^2] };
    }
}
