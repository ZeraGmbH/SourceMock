using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Provider;

namespace SourceApi.Actions.RestSource;

/// <summary>
/// Dosage algorithm which can by configured using a HTTP/REST connection.
/// </summary>
public interface IRestDosage : IDosage
{
    /// <summary>
    /// Configure the dosage connection once.
    /// </summary>
    /// <param name="endpoint">Endpoint of the remote dosage.</param>
    public void Initialize(RestConfiguration? endpoint);
}
