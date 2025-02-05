using ZERA.WebSam.Shared.Provider;
using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Configuration interface for an error calculator.
/// </summary>
public interface IErrorCalculatorInternal : IErrorCalculator
{
    /// <summary>
    /// Configure a brand new error calculator.
    /// </summary>
    /// <param name="position">Position of the test system.</param>
    /// <param name="configuration">Configuration to use.</param>
    /// <param name="services">Dependency injection.</param>
    Task InitializeAsync(int position, ErrorCalculatorConfiguration configuration, IServiceProvider services);

    /// <summary>
    /// Release all resources.
    /// </summary>
    void Destroy();
}
