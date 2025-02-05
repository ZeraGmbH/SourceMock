using WatchDogApi.Models;

namespace WatchDogApi.Services;

/// <summary>
/// WatchDog service
/// </summary>
public class WatchDogExecuter(IHttpClientFactory http) : IWatchDogExecuter
{
    /// <inheritdoc/>
    public List<string> BuildHttpEndpointList(string ip, int channelCount)
    {
        List<string> returnList = [];

        for (int i = 1; i <= channelCount; i++)
            returnList.Add("http://" + ip + "/cgi-bin/refreshpage" + i + ".asp");

        return returnList;
    }

    ///<inheritdoc/>
    public async Task<bool> QueryWatchDogSingleEndpointAsync(string endpoint, TimeSpan? timeout)
    {
        var httpClient = http.CreateClient();

        // if (timeout.HasValue) httpClient.Timeout = timeout.Value;

        var response = await httpClient.GetStringAsync(endpoint);

        return response.Contains("IP-WatchDog");
    }
}
