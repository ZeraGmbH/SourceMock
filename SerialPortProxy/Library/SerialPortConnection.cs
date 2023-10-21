using System.Diagnostics;
using System.IO.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SerialPortProxy;

/// <summary>
/// Represents the minimal set of methods neccessary to communicate with a serial port.
/// </summary>
public interface ISerialPort : IDisposable
{
    /// <summary>
    /// Send a command to the device.
    /// </summary>
    /// <param name="command">Command string - &lt;CR> is automatically added.</param>
    void WriteLine(string command);

    /// <summary>
    /// Wait for the next string from the device.
    /// </summary>
    /// <returns>The requested string.</returns>
    string ReadLine();
}

/// <summary>
/// Physical serial port mapped to the ISerialPort interface.
/// </summary>
class PhysicalProxy : ISerialPort
{
    /// <summary>
    /// The physical connection.
    /// </summary>
    private readonly SerialPort _port;

    /// <summary>
    /// Initialiuze a new wrapper.
    /// </summary>
    /// <param name="port">Name of the serial port, e.g. COM3 for Windows
    /// or /dev/ttyUSB0 for a USB serial adapter on Linux.</param>
    public PhysicalProxy(string port)
    {
        /* Connected as required in the API documentation. MT3101_RS232_EXT_GB.pdf */
        _port = new SerialPort
        {
            BaudRate = 9600,
            DataBits = 8,
            NewLine = "\r",
            Parity = Parity.None,
            PortName = port,
            ReadTimeout = 500,
            StopBits = StopBits.Two,
            WriteTimeout = 500
        };

        /* Always open the connection immediatly. */
        _port.Open();
    }

    /// <inheritdoc/>
    public void Dispose() => _port.Dispose();

    /// <inheritdoc/>
    public string ReadLine() => _port.ReadLine();

    /// <inheritdoc/>
    public void WriteLine(string command) => _port.WriteLine(command);
}

/// <summary>
/// Queue entry for a single request to the device.
/// </summary>
public class SerialPortRequest
{
    /// <summary>
    /// Command string to send to the device - &lt;qCR> automatically added.
    /// </summary>
    public readonly string Command;

    /// <summary>
    /// Expected termination line.
    /// </summary>
    public readonly string End;

    /// <summary>
    /// Promise for the response of the device.
    /// </summary>
    public readonly TaskCompletionSource<string[]> Result = new();

    /// <summary>
    /// Initialize a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Final string from the device to end the request.</param>
    private SerialPortRequest(string command, string end)
    {
        Command = command;
        End = end;
    }

    /// <summary>
    /// Create a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Final string from the device to end the request.</param>
    /// <returns>The new request.</returns>
    public static SerialPortRequest Create(string command, string end) => new SerialPortRequest(command, end);
}


/// <summary>
/// Class to manage a sinle serial line connection.
/// </summary>
public class SerialPortConnection : IDisposable
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

    private readonly ILogger<SerialPortConnection> _logger;

    /// <summary>
    /// Initialize a serial connection manager.
    /// </summary>
    /// <param name="port">Serial port proxy - maybe physical or mocked.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <exception cref="ArgumentNullException">Proxy must not be null.</exception>
    private SerialPortConnection(ISerialPort port, ILogger<SerialPortConnection> logger)
    {
        if (port == null)
            throw new ArgumentNullException("port");

        _logger = logger ?? new NullLogger<SerialPortConnection>();
        _port = port;

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
    public static SerialPortConnection FromSerialPort(string port, ILogger<SerialPortConnection> logger) => new(new PhysicalProxy(port), logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="logger">Optional logging instance.</param>
    /// <typeparam name="T">Some mocked class implementing ISerialPort.</typeparam>
    /// <returns>The new connection.</returns>
    public static SerialPortConnection FromMock<T>(ILogger<SerialPortConnection> logger) where T : class, ISerialPort, new() => FromMock(typeof(T), logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="mockType">Some mocked class implementing ISerialPort.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The new connection.</returns>
    public static SerialPortConnection FromMock(Type mockType, ILogger<SerialPortConnection> logger) => FromPortInstance((ISerialPort)Activator.CreateInstance(mockType)!, logger);

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="port">Implementation to use.</param>
    /// <param name="logger">Optional logging instance.</param>
    /// <returns>The new connection.</returns>
    public static SerialPortConnection FromPortInstance(ISerialPort port, ILogger<SerialPortConnection> logger) => new(port, logger);

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
            /* To be very safe. */
            Debug.WriteLine(e);
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
            if (pending != null)
                foreach (var requests in pending)
                    foreach (var request in requests)
                    {
                        _logger.LogDebug("Cancel command {0}", request.Command);

                        request.Result.SetException(new OperationCanceledException());
                    }
        }
    }

    /// <summary>
    /// Add a command to be sent to the serial port to the queue.
    /// </summary>
    /// <param name="requests">The command to send to the device.</param>
    /// <exception cref="ArgumentNullException">Parameter must not be null.</exception>
    /// <returns>All lines sent from the device as a task.</returns>
    public Task<string[]>[] Execute(params SerialPortRequest[] requests)

    {
        if (requests == null)
            throw new ArgumentNullException("requests");

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
            _logger.LogDebug("Sending command {0}", request.Command);

            /* Send the command string to the device - <CR> is automatically added. */
            _port.WriteLine(request.Command);

            _logger.LogDebug("Command {0} accepted by device", request.Command);
        }
        catch (Exception e)
        {
            _logger.LogDebug("Command {0} rejected: {1}", request.Command, e);

            /* Unable to sent the command - report error to caller. */
            request.Result.SetException(e);

            return false;
        }

        /* Collect response strings until expected termination string is detected. */
        for (var answer = new List<string>(); ;)
            try
            {
                /* Read a single response line from the device. */
                _logger.LogDebug("Wait for command {0} reply", request.Command);

                var reply = _port.ReadLine();

                _logger.LogDebug("Got reply {1} for command {0}", request.Command, reply);

                /* If a device response ends with NAK there are invalid arguments. */
                if (reply.EndsWith("NAK"))
                {
                    _logger.LogDebug("Command {0} reported NAK", request.Command);

                    request.Result.SetException(new ArgumentException(request.Command));

                    return false;
                }

                /* Always remember the reply - even the terminating string. */
                answer.Add(reply);

                /* If the terminating string is detected the reply from the device is complete. */
                if (reply == request.End)
                {
                    _logger.LogDebug("Command {0} finished, replies: {1}", request.Command, answer.Count());

                    /* Set the task result to all strings collected and therefore finish the task with success. */
                    request.Result.SetResult(answer.ToArray());

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug("Reading command {0} reply failed: {1}", request.Command, e);

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
            _logger.LogDebug("Starting transaction processing, commands: {0}", requests.Length);

            foreach (var request in requests)
                if (!ExecuteCommand(request))
                    break;
        }
    }
}
