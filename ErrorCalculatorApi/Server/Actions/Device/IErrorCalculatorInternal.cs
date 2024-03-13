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
    /// <param name="configuration">Configuration to use.</param>
    /// <param name="services">Dependency injection.</param>
    Task Initialize(ErrorCalculatorConfiguration configuration, IServiceProvider services);

    /// <summary>
    /// Release all resources.
    /// </summary>
    void Destroy();
}
