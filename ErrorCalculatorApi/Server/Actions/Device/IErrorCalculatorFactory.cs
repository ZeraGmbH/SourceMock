using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.Provider;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Factory to create error calculators from configuration.
/// </summary>
public interface IErrorCalculatorFactory
{
    /// <summary>
    /// Create a new error calculator using a given configuration.
    /// </summary>
    /// <param name="position">Position of the test system.</param>
    /// <param name="configuration">Configuration to use.</param>
    /// <returns>Error calculator configured as requested.</returns>
    Task<IErrorCalculator> CreateAsync(int position, ErrorCalculatorConfiguration configuration);
}
