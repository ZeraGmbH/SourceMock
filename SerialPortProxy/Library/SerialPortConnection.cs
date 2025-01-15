using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace SerialPortProxy;

/// <summary>
/// Use for internal testings,
/// /// </summary>
public interface ISerialPortConnectionMock : ISerialPortConnection
{
    /// <summary>
    /// Report the corresponding port.
    /// </summary>
    ISerialPort Port { get; }
}

/// <summary>
/// Class to manage a single serial line connection.
/// </summary>
public partial class SerialPortConnection : ISerialPortConnectionMock
{
    private class Executor(SerialPortConnection port, InterfaceLogEntryConnection connection) : ISerialPortConnectionExecutor
    {
        /// <inheritdoc/>
        public Task<string[]>[] ExecuteAsync(IInterfaceLogger logger, params SerialPortRequest[] requests)
            => port.ExecuteAsync(logger.CreateConnection(connection), requests);

        public Task<T> RawExecuteAsync<T>(IInterfaceLogger logger, Func<ISerialPort, IInterfaceConnection, T> algorithm)
            => port.ExecuteAsync(logger.CreateConnection(connection), algorithm);
    }

    private abstract class QueueItem
    {
        public abstract void Execute(SerialPortConnection connection);

        public abstract void Discard(SerialPortConnection connection);
    }

    /// <summary>
    /// Read timeout - may be changed when running in unit test mode.
    /// </summary>
    private static int? UnitTest;

    /// <summary>
    /// Enter unit test mode.
    /// </summary>
    /// <param name="timeout">Timeout to use (in milliseconds).</param>
    public static void ActivateUnitTestMode(int timeout) => UnitTest = timeout;

    /// <summary>
    /// Non thread-safe queue - may use ConcurrentQueue or Producer/Consumer pattern with integrated locking.
    /// </summary>
    private readonly Queue<QueueItem> _queue = new();

    /// <summary>
    /// The physical connection to a serial port.
    /// </summary>
    private readonly ISerialPort _port;

    /// <summary>
    /// Unset a soon as the connection is disposed - _executer thread will terminate.
    /// </summary>
    private bool _running = true;

    /// <summary>
    /// All queued incoming data.
    /// </summary>
    private readonly Queue<string> _incoming = [];

    /// <summary>
    /// Logger to use, esp. for communication tracing.
    /// </summary>
    private readonly ILogger<ISerialPortConnection> _logger;

    /// <summary>
    /// All registered out-of-band message handlers.
    /// </summary>
    private readonly ConcurrentBag<Tuple<Regex, Action<Match>>> _handlers = [];

    /// <summary>
    /// Target identification for interface logging.
    /// </summary>
    private readonly InterfaceLogEntryTargetConnection _target;

    /// <summary>
    /// Timeout (in Milliseconds) to wait on input after sending a command.
    /// </summary>
    private readonly int ReadTimeout;

    /// <inheritdoc/>
    public ISerialPort Port => _port;

    private readonly ICancellationService? _cancel;

    /// <summary>
    /// Initialize a serial connection manager.
    /// </summary>
    /// <param name="port">Serial port proxy - maybe physical or mocked.</param>
    /// <param name="target">Target identification for interface logging.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Service to ask for termination of the current operation.</param>
    /// <exception cref="ArgumentNullException">Proxy must not be null.</exception>
    private SerialPortConnection(
        ISerialPort port,
        InterfaceLogEntryTargetConnection target,
        ILogger<ISerialPortConnection> logger,
        bool enableReader,
        int? readTimeout,
        ICancellationService? cancel)
    {
        ReadTimeout = readTimeout ?? 30000;

        _cancel = cancel;
        _logger = logger ?? new NullLogger<SerialPortConnection>();
        _port = port ?? throw new ArgumentNullException(nameof(port));
        _target = target;

        /* Create and start a thread handling requests to the serial port. */
        new Thread(ProcessFromQueue).Start();

        /* Create and start a thread handling input from the serial port. */
        if (enableReader) new Thread(ProcessInput).Start();
    }

    /// <summary>
    /// Create a new connection using a physical connection to a serial port.
    /// </summary>
    /// <param name="port">Name of the serial port, e.g. COM3 for Windows
    /// or /dev/ttyUSB0 for a USB serial adapter on Linux.</param>
    /// <param name="options">Additional options.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The brand new connection.</returns>
    public static ISerialPortConnection FromSerialPort(string port, SerialPortOptions? options, ILogger<ISerialPortConnection> logger, bool enableReader = true, ICancellationService? cancel = null)
        => FromPortInstance(new PhysicalPortProxy(port, options), new() { Protocol = InterfaceLogProtocolTypes.Com, Endpoint = port }, logger, enableReader, options?.ReadTimeout, cancel);

    /// <summary>
    /// Create a new connection based on a TCP-to-Serial passthrouh connection.
    /// </summary>
    /// <param name="serverAndPort">Name of the server to connect including the port - separated by colons.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The brand new connection.</returns>
    public static ISerialPortConnection FromNetwork(string serverAndPort, ILogger<ISerialPortConnection> logger, bool enableReader = true, int? readTimeout = null, ICancellationService? cancel = null)
        => FromPortInstance(new TcpPortProxy(serverAndPort, readTimeout), new() { Protocol = InterfaceLogProtocolTypes.Com, Endpoint = serverAndPort }, logger, enableReader, readTimeout, cancel);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="logger">Optional logging instance.</param>
    /// <typeparam name="T">Some mocked class implementing ISerialPort.</typeparam>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromMock<T>(ILogger<ISerialPortConnection> logger, bool enableReader = true, int? readTimeout = null, ICancellationService? cancel = null) where T : class, ISerialPort, new()
        => FromMock(typeof(T), logger, enableReader, readTimeout, cancel);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="port">Implementation to use.</param>
    /// <param name="target">Target identification for interface logging.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The new connection.</returns>
    private static ISerialPortConnection FromPortInstance(ISerialPort port, InterfaceLogEntryTargetConnection target, ILogger<ISerialPortConnection> logger, bool enableReader = true, int? readTimeout = null, ICancellationService? cancel = null)
        => new SerialPortConnection(port, target, logger, enableReader, readTimeout, cancel);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="mockType">Some mocked class implementing ISerialPort.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromMock(Type mockType, ILogger<ISerialPortConnection> logger, bool enableReader = true, int? readTimeout = null, ICancellationService? cancel = null)
        => FromMockedPortInstance((ISerialPort)Activator.CreateInstance(mockType)!, logger, enableReader, readTimeout, cancel);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="port">Implementation to use.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <param name="enableReader">Unset to disable the input reader.</param>
    /// <param name="readTimeout">Timeout (in Milliseconds) to wait on input after sending a command.</param>
    /// <param name="cancel">Manage script execution cancel to allow early abort.</param>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromMockedPortInstance(ISerialPort port, ILogger<ISerialPortConnection> logger, bool enableReader = true, int? readTimeout = null, ICancellationService? cancel = null)
        => FromPortInstance(port, new() { Protocol = InterfaceLogProtocolTypes.Mock, Endpoint = "mocked" }, logger, enableReader, readTimeout, cancel);

    /// <summary>
    /// On dispose the serial connection and the ProcessFromQueue thread are terminated.
    /// </summary>
    public void Dispose()
    {
        /* Indicated this connection as terminated. */
        _running = false;

        /* Close the physical connection. */
        try
        {
            _port.Dispose();
        }
        catch (Exception e)
        {
            _logger.LogWarning("Unable to close port: {Exception}", e);
        }

        /* Get exclusive access to the queue. */
        lock (_queue)
        {
            /* Find all outstanding requests. */
            var items = _queue.ToArray();

            /* Empty the queue. */
            _queue.Clear();

            /* Wake up the _executer thread which will test the _running state. */
            Monitor.Pulse(_queue);

            /* If there are any outstand requests notify the corresponding clients - callers of Execute. */
            foreach (var item in items) item.Discard(this);
        }
    }

    /// <inheritdoc/>
    public ISerialPortConnectionExecutor CreateExecutor(InterfaceLogSourceTypes type, string id)
        => new Executor(this, new()
        {
            Protocol = _target.Protocol,
            Endpoint = _target.Endpoint,
            WebSamType = type,
            WebSamId = id
        });

    /// <summary>
    /// Thread method processing all queue entries as long as connection is active (_running).
    /// </summary>
    private void ProcessFromQueue()
    {
        /* Done if no longer running - i.e. connection is disposed. */
        while (_running)
        {
            /* Always clear the input queue periodically. */
            if (!UnitTest.HasValue)
                lock (_incoming)
                    _incoming.Clear();

            QueueItem? entry;

            /* Must have exclusive access to the queue to avoid data corruption. */
            lock (_queue)
            {
                /* Since we waited on the lock we should retest the running state. */
                if (!_running)
                    return;

                /* Try to get the first (oldest) entry from the queue. */
                if (!_queue.TryDequeue(out entry))
                {
                    _logger.LogDebug("Queue is empty, waiting for next request");

                    /* If queue is empty wait until someone intentionally wakes us up (Monitor.Pulse) to avoid unnecessary processings. */
                    Monitor.Wait(_queue, 15000);

                    continue;
                }
            }

            /* Process the transaction until finished or some request failed - important: ExecuteCommand MUST NOT throw an exception. */
            entry.Execute(this);
        }
    }

    /// <summary>
    /// Get the next line from the input queue.
    /// </summary>
    /// <remarks>Use timeout with care since it can block the server.</remarks>
    /// <param name="timeout">Maximum timeout (in milliseconds) to wait for an answer.</param>
    /// <returns>The next line.</returns>
    private string ReadInput(int? timeout)
    {
        /* Time limit for the operation. */
        var end = DateTime.UtcNow.AddMilliseconds(timeout ?? UnitTest ?? ReadTimeout);

        lock (_incoming)
            for (; ; )
            {
                /* Maybe data is already available. */
                if (_incoming.TryDequeue(out var line))
                    return line;

                /* Wait for new data - respecting timeout and cancellation. */
                if (!Monitor.Wait(_incoming, 100))
                    if (DateTime.UtcNow >= end)
                        throw new TimeoutException("no reply from serial port");
                    else
                        _cancel?.ThrowIfCancellationRequested();
            }
    }

    /// <inheritdoc/>
    public void RegisterEvent(Regex pattern, Action<Match> handler) =>
        _handlers.Add(Tuple.Create(pattern, handler));

    /// <summary>
    /// Process data from the serial port connection.
    /// </summary>
    private void ProcessInput()
    {
        while (_running)
            try
            {
                /* Try to read the next line - may report some timeout. */
                var line = _port.ReadLine() ?? string.Empty;

                /* Add to queue. */
                lock (_incoming)
                {
                    _incoming.Enqueue(line);

                    /* Wakeup pending reader - if any. */
                    Monitor.PulseAll(_incoming);
                }

                /* Find all out of bound handlers. */
                foreach (var (pattern, handler) in _handlers)
                {
                    try
                    {
                        /* Analyse and process. */
                        var match = pattern.Match(line);

                        if (match.Success) handler(match);
                    }
                    catch (Exception e)
                    {
                        /* Really bad, may decrease overall performance. */
                        _logger.LogCritical("Failed to process reply {Reply} on pattern {Pattern}: {Exception}", line, pattern, e);
                    }
                }
            }
            catch (Exception)
            {
                /* Ignore any error. */
            }
    }
}
