using SourceMock.Model;

namespace SourceMock.Actions.Source
{
    /// <summary>
    /// Interface of a class that simbulates the behaviour of a ZERA source.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Sets a loadpoint which will be put to action with the next call of <see cref="TurnOn"/>.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The corresponding value of <see cref="SourceResult"/> with regard to the success of the operation.</returns>
        public SourceResult SetLoadpoint(Loadpoint loadpoint);

        /// <summary>
        /// Turns on the source with the previously set loadpoint.
        /// </summary>
        /// <returns>The corresponding value of <see cref="SourceResult"/> with regard to the success of the operation.</returns>
        public SourceResult TurnOn();

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The corresponding value of <see cref="SourceResult"/> with regard to the success of the operation.</returns>
        public SourceResult TurnOff();

        /// <summary>
        /// Gets the loadpoint that would be put to action if <see cref="TurnOn"/> would be called.
        /// </summary>
        /// <returns>The loadpoint, null if none was set.</returns>
        public Loadpoint? GetNextLoadpoint();

        /// <summary>
        /// Gets the currently set loadpoint.
        /// </summary>
        /// <returns>The loadpoint, null if none was set.</returns>
        public Loadpoint? GetCurrentLoadpoint();
    }
}