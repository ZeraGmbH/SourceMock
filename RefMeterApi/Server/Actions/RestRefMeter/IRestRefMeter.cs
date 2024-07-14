using RefMeterApi.Actions.Device;
using ZERA.WebSam.Shared.Models;

namespace RefMeterApi.Actions.RestSource;

/// <summary>
/// Reference meter which can by configured using a HTTP/REST connection.
/// </summary>
public interface IRestRefMeter : IRefMeter
{
    /// <summary>
    /// Configure the reference meter connection once.
    /// </summary>
    /// <param name="endpoint">Endpoint of the remote source.</param>
    public void Initialize(RestConfiguration? endpoint);
}
