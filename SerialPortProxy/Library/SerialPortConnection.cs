using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SerialPortProxy;

/// <summary>
/// Class to manage a single serial line connection.
/// </summary>
public class SerialPortConnection : ISerialPortConnection
{
    /// <summary>
    /// Non thread-safe queue - may use ConcurrentQueue or Producer/Consumer pattern with integrated locking.
    /// </summary>
    private readonly Queue<SerialPortRequest[]> _queue = new();

    /// <summary>
    /// The physical connection to a serial port.
    /// </summary>
    private readonly ISerialPort _port;

    /// <summary>
    /// Unset a soon as the connection is disposed - _executer thread will terminate.
    /// </summary>
    private bool _running = true;

    /// <summary>
    /// Logger to use, esp. for communication tracing.
    /// </summary>
    private readonly ILogger<ISerialPortConnection> _logger;

    /// <summary>
    /// Initialize a serial connection manager.
    /// </summary>
    /// <param name="port">Serial port proxy - maybe physical or mocked.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <exception cref="ArgumentNullException">Proxy must not be null.</exception>
    private SerialPortConnection(ISerialPort port, ILogger<ISerialPortConnection> logger)
    {
        _port = port ?? throw new ArgumentNullException(nameof(port));
        _logger = logger ?? new NullLogger<SerialPortConnection>();

        /* Create and start a thread handling requests to the serial port. */
        var executor = new Thread(ProcessFromQueue);

        executor.Start();
    }

    /// <summary>
    /// Create a new connection using a physical connection to a serial port.
    /// </summary>
    /// <param name="port">Name of the serial port, e.g. COM3 for Windows
    /// or /dev/ttyUSB0 for a USB serial adapter on Linux.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The brand new connection.</returns>
    public static ISerialPortConnection FromSerialPort(string port, ILogger<ISerialPortConnection> logger) => FromPortInstance(new PhysicalPortProxy(port), logger);

    /// <summary>
    /// Create a new connection based on a TCP-to-Serial passthrouh connection.
    /// </summary>
    /// <param name="serverAndPort">Name of the server to connect including the port - separated by colons.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The brand new connection.</returns>
    public static ISerialPortConnection FromNetwork(string serverAndPort, ILogger<ISerialPortConnection> logger) => FromPortInstance(new TcpPortProxy(serverAndPort), logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="logger">Optional logging instance.</param>
    /// <typeparam name="T">Some mocked class implementing ISerialPort.</typeparam>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromMock<T>(ILogger<ISerialPortConnection> logger) where T : class, ISerialPort, new() => FromMock(typeof(T), logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="mockType">Some mocked class implementing ISerialPort.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromMock(Type mockType, ILogger<ISerialPortConnection> logger) => FromPortInstance((ISerialPort)Activator.CreateInstance(mockType)!, logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="port">Implementation to use.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The new connection.</returns>
    public static ISerialPortConnection FromPortInstance(ISerialPort port, ILogger<ISerialPortConnection> logger) => new SerialPortConnection(port, logger);

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
            _logger.LogWarning($"Unable to close port: {e}");
        }

        /* Get exclusive access to the queue. */
        lock (_queue)
        {
            /* Find all outstanding requests. */
            var pending = _queue.ToArray();

            /* Empty the queue. */
            _queue.Clear();

            /* Wake up the _executer thread which will test the _running state. */
            Monitor.Pulse(_queue);

            /* If there are any outstand requests notify the corresponding clients - callers of Execute. */
            foreach (var requests in pending)
                foreach (var request in requests)
                {
                    _logger.LogWarning($"Cancel command {request.Command}");

                    request.Result.SetException(new OperationCanceledException());
                }
        }
    }

    /// <inheritdoc/>
    public Task<string[]>[] Execute(params SerialPortRequest[] requests)
    {
        if (requests == null)
            throw new ArgumentNullException(nameof(requests));

        /* Since we are expecting multi-threaded access lock the queue. */
        lock (_queue)
        {
            /* Queue is locked, we have exclusive access and can now safely add the entry. */
            if (requests.Length > 0)
                _queue.Enqueue(requests);

            /* If queue executer thread is waiting (Monitor.Wait) for new entries wake it up for immediate processing the new entry. */
            Monitor.Pulse(_queue);
        }

        /* Report the task related with the result promise. */
        return requests.Select(request => request.Result.Task).ToArray();
    }

    /// <summary>
    /// Executes a single command.
    /// </summary>
    /// <param name="request">Describes the request.</param>
    /// <returns>Gesetzt, wenn die Ausführung erfolgreich war.</returns>
    private bool ExecuteCommand(SerialPortRequest request)
    {
        try
        {
            _logger.LogDebug($"Sending command {request.Command}");

            /* Send the command string to the device - <CR> is automatically added. */
            _port.WriteLine(request.Command);

            _logger.LogDebug($"Command {request.Command} accepted by device");
        }
        catch (Exception e)
        {
            _logger.LogError($"Command {request.Command} rejected: {e}");

            /* Unable to sent the command - report error to caller. */
            request.Result.SetException(e);

            return false;
        }

        /* Collect response strings until expected termination string is detected. */
        for (var answer = new List<string>(); ;)
            try
            {
                /* Read a single response line from the device. */
                _logger.LogDebug($"Wait for command {request.Command} reply");

                var reply = _port.ReadLine();

                _logger.LogDebug($"Got reply {reply} for command {request.Command}");

                /* If a device response ends with NAK there are invalid arguments. */
                if (reply.EndsWith("NAK"))
                {
                    _logger.LogError($"Command {request.Command} reported NAK");

                    request.Result.SetException(new ArgumentException(request.Command));

                    return false;
                }

                /* Error handling for ERR commands. */
                if (reply.Contains("ER-") || reply.Contains("ERR-") || reply.Contains("ERROR"))
                {
                    _logger.LogError($"Command {request.Command} reported ERROR {reply}");

                    request.Result.SetException(new ArgumentException(request.Command));

                    return false;
                }

                /* Always remember the reply - even the terminating string. */
                answer.Add(reply);

                /* If the terminating string is detected the reply from the device is complete. */
                if (request.Match(reply))
                {
                    _logger.LogDebug($"Command {request.Command} finished, replies: {answer.Count()}");

                    /* Set the task result to all strings collected and therefore finish the task with success. */
                    request.Result.SetResult(answer.ToArray());

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Reading command {request.Command} reply failed: {e}");

                /* 
                    If it is not possible to read something from the device report exception to caller. 
                    In case the device does not recognize the command it will not respond anything. Then
                    the read method call will throw a time exception as configured in the constructor.
                */
                request.Result.SetException(e);

                return false;
            }
    }

    /// <summary>
    /// Thread method processing all queue entries as long as connection is active (_running).
    /// </summary>
    private void ProcessFromQueue()
    {
        /* Done if no longer running - i.e. connection is disposed. */
        while (_running)
        {
            SerialPortRequest[]? requests;

            /* Must have exclusive access to the queue to avoid data corruption. */
            lock (_queue)
            {
                /* Since we waited on the lock we should retest the running state. */
                if (!_running)
                    return;

                /* Try to get the first (oldest) entry from the queue. */
                if (!_queue.TryDequeue(out requests))
                {
                    _logger.LogDebug("Queue is empty, waiting for next request");

                    /* If queue is empty wait until someone intentionally wakes us up (Monitor.Pulse) to avoid unnecessary processings. */
                    Monitor.Wait(_queue);

                    continue;
                }
            }

            /* Process the transaction until finished or some request failed - important: ExecuteCommand MUST NOT throw an exception. */
            _logger.LogDebug($"Starting transaction processing, commands: {requests.Length}");

            var failed = false;

            foreach (var request in requests)
                if (failed)
                    request.Result.SetException(new OperationCanceledException("previous command failed"));
                else
                    failed = !ExecuteCommand(request);
        }
    }
}
