using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Provider;
using ZeraDevices.ErrorCalculator.STM;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Factory for error calculators.
/// </summary>
/// <param name="services">Dependeny injection.</param>
public class ErrorCalculatorFactory(IServiceProvider services) : IErrorCalculatorFactory
{
    /// <inheritdoc/>
    public async Task<IErrorCalculator> CreateAsync(int position, ErrorCalculatorConfiguration configuration)
    {
        /* Create the implementation. */
        var ec = services.GetRequiredKeyedService<IErrorCalculatorSetup>(configuration.Protocol);

        /* Configure it. */
        try
        {
            await ec.InitializeAsync(position, configuration, services);

            /* Report configured instance. */
            return ec;
        }
        catch (Exception)
        {
            ec.Destroy();

            throw;
        }
    }
}
