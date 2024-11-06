using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;
using ZERA.WebSam.Shared.DomainSpecific;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Interface of a class that simbulates the behaviour of a ZERA source.
    /// </summary>
    public interface ISource : IDosage
    {
        /// <summary>
        /// Set if the source is fully configured and can be used.
        /// </summary>
        Task<bool> GetAvailableAsync(IInterfaceLogger logger);

        /// <summary>
        /// Gets the capabilities of this source.
        /// </summary>
        /// <returns>The corresponding <see cref="SourceCapabilities"/>-Object for this source.</returns>
        public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger logger);

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
        public Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger logger);

        /// <summary>
        /// Reports information on the active loadpoint.
        /// </summary>
        public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger logger);
    }
}