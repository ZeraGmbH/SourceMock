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
    }

    /// <summary>
    /// Possible results of source operations.
    /// /// </summary>
    public enum SourceResult
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        SUCCESS,
        /// <summary>
        /// The loadpoint that was tried to be set has a different number of phases than what this source can provide.
        /// </summary>
        LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES,
        /// <summary>
        /// The source was tried to be turned on without a soadpoint set with <see cref="ISource.SetLoadpoint"/> before.
        /// </summary>
        NO_LOADPOINT_SET
    }
}