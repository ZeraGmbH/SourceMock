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
    /// <param name="endPoint">Endpoint of the remote server.</param>
    public void Initialize(RestConfiguration endPoint);
}
