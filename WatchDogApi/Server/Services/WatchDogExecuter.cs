using WatchDogApi.Models;

namespace WatchDogApi.Services;

/// <summary>
/// WatchDog service
/// </summary>
public class WatchDogExecuter(IHttpClientFactory http) : IWatchDogExecuter
{
    ///<inheritdoc/>
    public async Task<bool> QueryWatchDogAsync(string endpoint)
    {
        var httpClient = http.CreateClient();

        var response = await httpClient.GetStringAsync(endpoint);

        return response.Contains("IP-WatchDog");
    }
}
