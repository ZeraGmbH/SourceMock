using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WatchDogApi.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace WatchDogApi.Actions;

/// <summary>
/// 
/// </summary>
public class WatchDog : IWatchDog, IDisposable
{
    private readonly Object _poller = new();

    private bool _running = true;

    private readonly ILogger<WatchDog> _logger;

    private readonly IServiceProvider _services;

    private readonly WatchDogConfiguration _config;

    private readonly IWatchDogExecuter _watchdogExecuter;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="services"></param>
    /// <param name="watchdogExecuter"></param>
    public WatchDog(WatchDogConfiguration config, IServiceProvider services, IWatchDogExecuter watchdogExecuter)
    {
        _config = config;
        _logger = services.GetRequiredService<ILogger<WatchDog>>();
        _services = services;
        _watchdogExecuter = watchdogExecuter;

        Task.Factory.StartNew(Poller);
    }

    private async Task Poller()
    {
        using var scope = _services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<IInterfaceLogger>();

        var logConnection = logger.CreateConnection(
            new InterfaceLogEntryConnection
            {
                Endpoint = _config.EndPoint ?? "n/a",
                Protocol = InterfaceLogProtocolTypes.Http,
                WebSamType = InterfaceLogSourceTypes.WatchDog,
            });

        while (_running)
        {
            // POLL
            await PollAsync(logConnection);

            // Wait for next POLL TIME (5000ms).
            lock (_poller)
                if (_running)
                    Monitor.Wait(_poller, 5000);
        }
    }

    private async Task PollAsync(IInterfaceConnection logConnection)
    {
        try
        {
            /* Prepare logging. */
            var requestId = Guid.NewGuid().ToString();
            var sendEntry = logConnection.Prepare(new() { Outgoing = true, RequestId = requestId });

            var sendInfo = new InterfaceLogPayload()
            {
                Encoding = InterfaceLogPayloadEncodings.Raw,
                Payload = "",
                PayloadType = ""
            };

            var receiveEntry = logConnection.Prepare(new() { Outgoing = false, RequestId = requestId });
            var receiveInfo = new InterfaceLogPayload()
            {
                Encoding = InterfaceLogPayloadEncodings.Raw,
                Payload = "",
                PayloadType = ""
            };

            try
            {
                ArgumentException.ThrowIfNullOrEmpty(_config.EndPoint);
                sendInfo.Payload = "URL: " + _config.EndPoint;
                if (!await _watchdogExecuter.QueryWatchDogAsync(_config.EndPoint))
                    throw new InvalidOperationException("Site reachable, but is not WatchDog!");
                receiveInfo.Payload = "WatchDog Polled.";
            }
            catch (Exception e)
            {
                receiveInfo.TransferException = e.Message;
            }
            finally
            {
                sendEntry.Finish(sendInfo);
                receiveEntry.Finish(receiveInfo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("unable to poll: {Exception}", e.Message);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_poller)
        {
            _running = false;

            Monitor.Pulse(_poller);
        }
    }
}