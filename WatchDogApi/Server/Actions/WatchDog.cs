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
    private readonly Thread _poller;

    private bool _running = true;

    private readonly ILogger<WatchDog> _logger;

    private readonly IServiceProvider _services;

    private readonly WatchDogConfiguration _config;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <param name="services"></param>
    public WatchDog(WatchDogConfiguration config, IServiceProvider services)
    {
        _config = config;
        _logger = services.GetRequiredService<ILogger<WatchDog>>();
        _services = services;

        _poller = new Thread(Poller);

        _poller.Start();
    }

    private void Poller()
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
            Poll(logConnection);

            // Wait for next POLL TIME (5000ms).
            lock (_poller)
                if (_running)
                    Monitor.Wait(_poller, 5000);
        }
    }

    private void Poll(IInterfaceConnection logConnection)
    {
        try
        {
            var requestId = Guid.NewGuid().ToString();
            var sendEntry = logConnection.Prepare(new() { Outgoing = true, RequestId = requestId });

            var sendInfo = new InterfaceLogPayload()
            {
                Encoding = InterfaceLogPayloadEncodings.Raw,
                Payload = "HTTP",
                PayloadType = ""
            };

            try
            {
                throw new NotImplementedException("connection refused");
            }
            catch (Exception e)
            {
                sendInfo.TransferException = e.Message;
            }
            finally
            {
                sendEntry.Finish(sendInfo);
            }

            /* Prepare logging. */
            var receiveEntry = logConnection.Prepare(new() { Outgoing = false, RequestId = requestId });
            var receiveInfo = new InterfaceLogPayload() { Encoding = InterfaceLogPayloadEncodings.Raw, Payload = "", PayloadType = "" };

            receiveInfo.TransferException = new NotImplementedException("not yet implemented").Message;

            receiveEntry.Finish(receiveInfo);
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