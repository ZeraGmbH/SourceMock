namespace SourceMock.Actions.Source
{
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