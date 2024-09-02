using ErrorCalculatorApi.Models;

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
    Task<IErrorCalculatorInternal> CreateAsync(int position, ErrorCalculatorConfiguration configuration);
}
