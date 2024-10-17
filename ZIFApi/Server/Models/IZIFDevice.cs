using ZERA.WebSam.Shared.Models.Logging;

namespace ZIFApi.Models;

/// <summary>
/// Interface provided by all ZIF implementations.
/// </summary>
public interface IZIFDevice
{
    /// <summary>
    /// Execute a single command.
    /// </summary>
    /// <param name="cmd">Command to execute.</param>
    /// <param name="logger"></param>
    /// <typeparam name="TResponse">Type of the response.</typeparam>
    /// <returns>Response of the command.</returns>
    Task<TResponse> Execute<TResponse>(Command<TResponse> cmd, IInterfaceLogger logger) where TResponse : Response;
}