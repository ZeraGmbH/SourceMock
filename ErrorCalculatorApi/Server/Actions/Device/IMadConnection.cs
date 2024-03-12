using System.Xml;
using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Implementation for MAD XML communication.
/// </summary>
public interface IMadConnection : IDisposable
{
    /// <summary>
    /// Configure the connection.
    /// </summary>
    /// <param name="config">Configuration to use.</param>
    public Task Initialize(ErrorCalculatorConfiguration config);

    /// <summary>
    /// Execute a single XML request.
    /// </summary>
    /// <param name="request">Request to send.</param>
    /// <returns>Response received.</returns>
    public Task<XmlDocument> Execute(XmlDocument request);
}
