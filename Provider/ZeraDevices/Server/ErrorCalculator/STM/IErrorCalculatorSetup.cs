using ZERA.WebSam.Shared.Models.ErrorCalculator;
using ZERA.WebSam.Shared.Provider;

namespace ZeraDevices.ErrorCalculator.STM;

/// <summary>
/// Configuration interface for an error calculator.
/// </summary>
public interface IErrorCalculatorSetup : IErrorCalculator
{
    /// <summary>
    /// Configure a brand new error calculator.
    /// </summary>
    /// <param name="position">Position of the test system.</param>
    /// <param name="configuration">Endpoint to connect to.</param>
    /// <param name="services">Dependency injection.</param>
    Task InitializeAsync(int position, ErrorCalculatorConfiguration configuration, IServiceProvider services);

    /// <summary>
    /// Release all resources.
    /// </summary>
    void Destroy();
}
