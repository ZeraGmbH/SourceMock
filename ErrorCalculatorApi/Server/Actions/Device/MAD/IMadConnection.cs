using System.Xml;
using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.Models.Logging;

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
    /// <param name="webSamId">Unique identifier inside WebSam.</param>
    /// <param name="config">Configuration to use.</param>
    /// <param name="readTimeout">Timeout while receiving data - in milliseconds.</param>
    /// <param name="writeTimeout">Timeout while sending data - in milliseconds.</param>
    public Task InitializeAsync(string webSamId, ErrorCalculatorConfiguration config, int? readTimeout = null, int? writeTimeout = null);

    /// <summary>
    /// Execute a single XML request.
    /// </summary>
    /// <param name="logger">Protocol logger.</param>
    /// <param name="request">Request to send.</param>
    /// <param name="reply">Reply node to expect.</param>
    /// <returns>Response received.</returns>
    public Task<XmlDocument> ExecuteAsync(IInterfaceLogger logger, XmlDocument request, string reply);
}
