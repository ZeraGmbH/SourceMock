using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

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
        bool GetAvailable(IInterfaceLogger logger);

        /// <summary>
        /// Gets the capabilities of this source.
        /// </summary>
        /// <returns>The corresponding <see cref="SourceCapabilities"/>-Object for this source.</returns>
        public Task<SourceCapabilities> GetCapabilities(IInterfaceLogger logger);

        /// <summary>
        /// Sets a specified loadpoint imediatly.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
        public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint);

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
        public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger);

        /// <summary>
        /// Gets the currently set loadpoint.
        /// </summary>
        /// <returns>The loadpoint, null if none was set.</returns>
        public TargetLoadpoint? GetCurrentLoadpoint(IInterfaceLogger logger);

        /// <summary>
        /// Reports information on the active loadpoint.
        /// </summary>
        public LoadpointInfo GetActiveLoadpointInfo(IInterfaceLogger logger);
    }
}