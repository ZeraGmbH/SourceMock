using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

/// <summary>
/// MAD communication using a TCP channel.
/// </summary>
/// <param name="logger">Logging helper.</param>
public class MadTcpConnection(ILogger<MadTcpConnection> logger) : IMadConnection
{
    /// <summary>
    /// Convert endpoint from string to server name and port.
    /// </summary>
    private static readonly Regex parseEndpoint = new(@"^(.+):([0-9]+)$");

    /// <summary>
    /// Parsed server name or IP.
    /// </summary>
    private string server = null!;

    /// <summary>
    /// Parsed port number to use.
    /// </summary>
    private ushort port;

    /// <summary>
    /// Collect incoming TCP stream.
    /// </summary>
    private readonly List<byte> _collector = [];

    /// <summary>
    /// Last XML document collected from stream.
    /// </summary>
    private XmlDocument? _response;

    /// <summary>
    /// Make sure that only a single request is active at any time.
    /// </summary>
    private readonly object _sync = new();

    /// <summary>
    /// TCP/IP client connection to use.
    /// </summary>
    private TcpClient client = new() { SendTimeout = 5000, ReceiveTimeout = 5000 };

    /// <inheritdoc/>
    public bool Available => client.Connected;

    /// <summary>
    /// Set to terminate all outstanding operations.
    /// </summary>
    private readonly CancellationTokenSource _doneReader = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        /* Force reader task to terminate. */
        _doneReader.Cancel();

        /* Get rid of connection to device. */
        using (client)
            client = null!;
    }

    /// <summary>
    /// Connect to the device.
    /// </summary>
    /// <returns>Terminates after a successful connect.</returns>
    private async Task Connect()
    {
        /* Try to connect if we are not already connected. */
        for (; !client.Connected; Thread.Sleep(5000))
            try
            {
                /* Do a regular connect - will be cancelled on dispose. */
                await client.ConnectAsync(server, port, _doneReader.Token);

                /* Start with a fresh buffer. */
                _collector.Clear();
            }
            catch (Exception e)
            {
                /* On cancel just forward the exception. */
                if (_doneReader.IsCancellationRequested) throw;

                logger.LogError("Failed to connect to MAD server on {Address} - retry in 5s: {Exception}", client, e.Message);
            }
    }

    /// <summary>
    /// Process incoming data.
    /// </summary>
    private async void ProcessIncoming()
    {
        /* As long as the connection instance is active - use a buffer large enough to hold any XML response to improve performance. */
        for (var buffer = new byte[10000]; ;)
            try
            {
                /* Connect to the device - once. */
                await Connect();

                /* Read the next chunk of data. */
                var data = await client.GetStream().ReadAsync(buffer, _doneReader.Token);

                if (data < 1)
                    using (client)
                    {
                        /* Server may be down, force reconnect. */
                        client = new() { SendTimeout = 5000, ReceiveTimeout = 5000 };

                        continue;
                    }

                /* Merge in. */
                _collector.AddRange(buffer.Take(data));

                /* Check for complete XML - normally we receive only empty keep-alive messages.. */
                var all = Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_collector)).Trim();

                if (string.IsNullOrEmpty(all))
                    /* Get rid of keep-alive data. */
                    _collector.Clear();
                else
                    try
                    {
                        /* Parse the XML. */
                        var doc = new XmlDocument();

                        doc.LoadXml(all);

                        /* Eat it all. */
                        _collector.Clear();

                        /* Report it. */
                        lock (_collector)
                        {
                            _response = doc;

                            /* Hopefully someone is already waiting for the response. */
                            Monitor.Pulse(_collector);
                        }
                    }
                    catch (Exception)
                    {
                        /* XML not yet complete - does no really harm but better extend the buffer. */
                        if (all.Length > 1000000) _collector.Clear();
                    }
            }
            catch (OperationCanceledException)
            {
                /* Dispose has been called. */
                break;
            }
            catch (Exception e)
            {
                /* Normally this would be a disconnect and we retry to reconnect. */
                logger.LogError("Problem with MAD connection: {Exception}", e.Message);

                Thread.Sleep(1000);
            }
    }

    /// <inheritdoc/>
    public Task<XmlDocument> Execute(XmlDocument request, string reply) => Task.Run(() =>
    {
        /* Wait for connect. */
        for (var i = 100; !client.Connected && i-- > 0; Thread.Sleep(100))
            if (i == 0)
                throw new InvalidOperationException("no connection to error calculator");

        /* There can be at most one outstaning request at any time. */
        lock (_sync)
        {
            /* Send request. */
            client.GetStream().Write(Encoding.UTF8.GetBytes(request.OuterXml + "\n"));

            /* Wait for reply. */
            XmlDocument response;

            lock (_collector)
            {
                Monitor.Wait(_collector);

                response = _response!;
            }

            /* Check job status. */
            var jobInfo = response.SelectSingleNode(@"KMA_XML_0_01/kmaContainer/jobDetails");
            var jobCode = jobInfo?.SelectSingleNode("jobResult")?.InnerText?.Trim();

            if (jobCode != "OK") throw new InvalidOperationException("command execution failed");

            /* Check command status. */
            var resInfo = response.SelectSingleNode($"KMA_XML_0_01/kmaContainer/{reply}");
            var resCode = resInfo?.SelectSingleNode("cmdResCode")?.InnerText?.Trim();

            if (resCode != "OK") throw new InvalidOperationException("command execution failed");

            return response;
        }
    });

    /// <inheritdoc/>
    public Task Initialize(ErrorCalculatorConfiguration config)
    {
        /* Test endpoint. */
        var match = parseEndpoint.Match(config.Endpoint ?? string.Empty);

        if (match == null) throw new ArgumentException("invalid endpoint");

        /* Remember. */
        server = match.Groups[1].Value;
        port = ushort.Parse(match.Groups[2].Value);

        /* Start task to readout incoming data. */
        Task.Run(ProcessIncoming);

        return Task.CompletedTask;
    }
}
