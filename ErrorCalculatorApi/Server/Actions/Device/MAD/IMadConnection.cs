using System.Xml;
using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device.MAD;

/// <summary>
/// Implementation for MAD XML communication.
/// </summary>
public interface IMadConnection : IDisposable
{
    /// <summary>
    /// Set if the error calculator is fully configured and can be used.
    /// </summary>
    bool Available { get; }

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
