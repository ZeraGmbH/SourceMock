using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Factory for error calculators.
/// </summary>
/// <param name="services">Dependeny injection.</param>
public class ErrorCalculatorFactory(IServiceProvider services) : IErrorCalculatorFactory
{
    /// <inheritdoc/>
    public async Task<IErrorCalculatorInternal> CreateAsync(int position, ErrorCalculatorConfiguration configuration)
    {
        /* Create the implementation. */
        var ec = services.GetRequiredKeyedService<IErrorCalculatorInternal>(configuration.Protocol);

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
