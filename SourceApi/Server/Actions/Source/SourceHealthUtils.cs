using SourceApi.Model;
using ZERA.WebSam.Shared.Models.Logging;

namespace SourceApi.Actions.Source;

/// <summary>
/// Helper class to manipulate the loadpoint and remember who
/// did it. 
/// </summary>
public class SourceHealthUtils(ISource source) : ISourceHealthUtils
{
    /// <inheritdoc/>
    public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger logger)
    {
        return source.GetCurrentLoadpointAsync(logger);
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
    {
        return source.SetLoadpointAsync(logger, loadpoint);
    }

    /// <inheritdoc/>
    public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger)
    {
        return source.TurnOffAsync(logger);
    }
}