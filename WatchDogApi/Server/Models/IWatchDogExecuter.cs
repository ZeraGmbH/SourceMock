namespace WatchDogApi.Models
{
    /// <summary>
    /// Interface of the service executing watchdog requests
    /// </summary>
    public interface IWatchDogExecuter
    {
        /// <summary>
        /// Send out GET request to http watchdog endpoint
        /// </summary>
        /// <param name="endpoint">full http endpoint of watchdog</param>
        /// <param name="timeout">Optional timeout to wait for response.</param>
        /// <returns>success</returns>
        Task<bool> QueryWatchDogSingleEndpointAsync(string endpoint, TimeSpan? timeout = null);

        /// <summary>
        /// Get the list of endpoints to be queried for a specific ip
        /// </summary>
        /// <param name="ip">IP Address of watchDog</param>
        /// <param name="channelCount">Amount of channels to request</param>
        /// <returns>List of endpoints</returns>
        List<string> BuildHttpEndpointList(string ip, int channelCount);
    }
}