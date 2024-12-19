using System.Security.Claims;
using SourceApi.Model;
using ZERA.WebSam.Shared.Actions.User;
using ZERA.WebSam.Shared.Models.Logging;

namespace SourceApi.Actions.Source;

/// <summary>
/// Helper class to manipulate the loadpoint and remember who
/// did it. 
/// </summary>
public class SourceHealthUtils(ISource source, ICurrentUser user, SourceHealthUtils.State state) : ISourceHealthUtils
{
    public class State
    {
        /// <summary>
        /// The user who modified the loadpoint.
        /// </summary>
        public ClaimsPrincipal? User;
    }

    /// <inheritdoc/>
    public async Task<Tuple<TargetLoadpoint?, ClaimsPrincipal?>> GetCurrentLoadpointAsync(IInterfaceLogger logger)
        => Tuple.Create(await source.GetCurrentLoadpointAsync(logger), state.User);

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        var result = await source.SetLoadpointAsync(logger, loadpoint);

        if (result == SourceApiErrorCodes.SUCCESS) state.User = user.User;

        return result;
    }

    /// <inheritdoc/>
    public async Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        var result = await source.TurnOffAsync(logger);

        if (result == SourceApiErrorCodes.SUCCESS) state.User = user.User;

        return result;
    }
}