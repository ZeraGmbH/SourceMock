using ErrorCalculatorApi.Actions.Device;
using ZERA.WebSam.Shared.Models;

namespace ErrorCalculatorApi.Actions;

/// <summary>
/// Error calculator interface for external implementations
/// based on a REST interface.
/// </summary>
public interface IRestErrorCalculator : IErrorCalculator
{
    /// <summary>
    /// Configure the access to the point.
    /// </summary>
    /// <param name="endpoint">Endpoint to use.</param>
    public void Initialize(RestConfiguration? endpoint);
}