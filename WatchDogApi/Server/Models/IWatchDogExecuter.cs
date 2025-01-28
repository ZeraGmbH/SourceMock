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
        /// <returns>success</returns>
        Task<bool> QueryWatchDogAsync(string endpoint);
    }
}