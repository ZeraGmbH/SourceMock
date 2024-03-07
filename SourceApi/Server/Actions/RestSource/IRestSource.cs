using SharedLibrary.Models;
using SourceApi.Actions.Source;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Source which can by configured using a HTTP/REST connection.
/// </summary>
public interface IRestSource : ISource
{
    /// <summary>
    /// Configure the source connection once.
    /// </summary>
    /// <param name="sourceEndpoint">Endpoint of the remote source.</param>
    /// <param name="dosageEndpoint">Endpoint of the remote dosage.</param>
    public void Initialize(RestConfiguration sourceEndpoint, RestConfiguration dosageEndpoint);
}
