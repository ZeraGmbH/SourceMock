using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Source
{
    /// <summary>
    /// Interface of a class that simbulates the behaviour of a ZERA source.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Gets the capabilities of this source.
        /// </summary>
        /// <returns>The corresponding <see cref="SourceCapabilities"/>-Object for this source.</returns>
        public Task<SourceCapabilities> GetCapabilities();

        /// <summary>
        /// Sets a specified loadpoint imediatly.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The corresponding value of <see cref="SourceResult"/> with regard to the success of the operation.</returns>
        public Task<SourceResult> SetLoadpoint(Loadpoint loadpoint);

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The corresponding value of <see cref="SourceResult"/> with regard to the success of the operation.</returns>
        public Task<SourceResult> TurnOff();

        /// <summary>
        /// Gets the currently set loadpoint.
        /// </summary>
        /// <returns>The loadpoint, null if none was set.</returns>
        public Loadpoint? GetCurrentLoadpoint();
    }
}