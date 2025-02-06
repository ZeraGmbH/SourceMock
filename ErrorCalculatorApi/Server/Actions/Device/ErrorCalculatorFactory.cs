using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
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
        var ec1 = services.GetKeyedService<IErrorCalculatorInternalLegacy>(configuration.Protocol);

        if (ec1 != null)
            /* Configure it. */
            try
            {
                await ec1.InitializeAsync(position, configuration, services);

                /* Report configured instance. */
                return ec1;
            }
            catch (Exception)
            {
                ec1.Destroy();

                throw;
            }

        /* Create the implementation. */
        var ec2 = services.GetRequiredKeyedService<IErrorCalculatorInternal>(configuration.Protocol);

        /* Configure it. */
        try
        {
            await ec2.InitializeAsync(position, configuration.Endpoint, services);

            /* Report configured instance. */
            return ec2;
        }
        catch (Exception)
        {
            ec2.Destroy();

            throw;
        }
    }
}
