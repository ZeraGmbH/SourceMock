using ZERA.WebSam.Shared.Models.Logging;
using System.Security.Claims;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Provider;

namespace SourceApi.Actions.Source;

/// <summary>
/// Interface of a class that simbulates the behaviour of a ZERA source.
/// </summary>
public interface ISourceHealthUtils
{
    /// <summary>
    /// Sets a specified loadpoint imediatly.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="loadpoint">The loadpoint to be set.</param>
    /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
    public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint);

    /// <summary>
    /// Turns off the source.
    /// </summary>
    /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
    public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger);

    /// <summary>
    /// Gets the currently set loadpoint.
    /// </summary>
    /// <returns>The loadpoint, null if none was set.</returns>
    Task<Tuple<TargetLoadpoint?, ClaimsPrincipal?>> GetCurrentLoadpointAsync(IInterfaceLogger logger);

}
