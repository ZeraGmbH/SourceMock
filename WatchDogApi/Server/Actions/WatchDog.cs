using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WatchDogApi.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace WatchDogApi.Actions;

/// <summary>
/// 
/// </summary>
public class WatchDog : IWatchDog
{
    private readonly object _poller = new();

    private bool _running = true;

    private readonly ILogger<WatchDog> _logger;

    private readonly IServiceProvider _services;

    private WatchDogConfiguration _config;

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

        while (_running)
        {
            // POLL
            var config = _config;

            if (!string.IsNullOrEmpty(config.EndPoint))
            {
                var logConnection = logger.CreateConnection(
                    new InterfaceLogEntryConnection
                    {
                        Endpoint = config.EndPoint,
                        Protocol = InterfaceLogProtocolTypes.Http,
                        WebSamType = InterfaceLogSourceTypes.WatchDog,
                    });

                await PollAsync(config, logConnection);
            }

            // Wait for next POLL TIME, if not set use 15s but at least 1s.
            lock (_poller)
                if (_running && _config == config)
                    Monitor.Wait(_poller, Math.Max(1000, config.Interval ?? 15000));
        }
    }

    private async Task PollAsync(WatchDogConfiguration config, IInterfaceConnection logConnection)
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
                sendInfo.Payload = "URL: " + config.EndPoint;

                sendEntry.Finish(sendInfo);

                if (!await _watchdogExecuter.QueryWatchDogAsync(config.EndPoint!))
                    throw new InvalidOperationException("Site reachable, but is not WatchDog!");

                receiveInfo.Payload = "WatchDog Polled.";
            }
            catch (Exception e)
            {
                receiveInfo.TransferException = e.Message;
            }
            finally
            {
                receiveEntry.Finish(receiveInfo);
            }
        }
        catch (Exception e)
        {
            _logger.LogError("unable to poll: {Exception}", e.Message);
        }
    }

    /// <inheritdoc/>
    public void Terminate()
    {
        lock (_poller)
        {
            _running = false;

            Monitor.Pulse(_poller);
        }
    }

    /// <inheritdoc/>
    public void SetConfig(WatchDogConfiguration config)
    {
        lock (_poller)
        {
            _config = config;

            Monitor.Pulse(_poller);
        }
    }
}