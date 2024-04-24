using DutApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DutApi.Actions;

/// <summary>
/// Factory to create device under test connections.
/// </summary>
/// <param name="services">Dependency injection system.</param>
public class ConnectionFactory(IServiceProvider services) : IDeviceUnderTestFactory
{
    /// <inheritdoc/>
    public async Task<IDeviceUnderTest> Create(string webSamId, DutConnection connection, DutStatusRegisterInfo[] status)
    {
        /* Request from DI. */
        var dut = services.GetRequiredKeyedService<IDeviceUnderTestConnection>(DutProtocolTypes.SCPIOverTCP);

        try
        {
            /* Set it up and report configured instance. */
            await dut.Initialize(webSamId, connection, status);

            return dut;
        }
        catch (Exception)
        {
            /* Must release properly. */
            using (dut)
                throw;
        }

    }
}