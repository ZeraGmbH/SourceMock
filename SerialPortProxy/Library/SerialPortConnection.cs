using System.Diagnostics;
using System.IO.Ports;

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
    private readonly Queue<SerialPortRequest> _queue = new();

    /// <summary>
    /// The physical connection to a serial port.
    /// </summary>
    private readonly ISerialPort _port;

    /// <summary>
    /// Unset a soon as the connection is disposed - _executer thread will terminate.
    /// </summary>
    private bool _running = true;

    /// <summary>
    /// Initialize a serial connection manager.
    /// </summary>
    /// <param name="port">Serial port proxy - maybe physical or mocked.</param>
    private SerialPortConnection(ISerialPort port)
    {
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
    /// <returns>The brand new connection.</returns>
    public static SerialPortConnection FromSerialPort(string port) => new(new PhysicalProxy(port));

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <typeparam name="T">Some mocked class implementing ISerialPort.</typeparam>
    /// <returns>The new connection.</returns>
    public static SerialPortConnection FromMock<T>() where T : class, ISerialPort, new() => FromMock(typeof(T));

    /// <summary>
    /// Create a new mocked based connection.
    /// </summary>
    /// <param name="mockType">Some mocked class implementing ISerialPort.</param>
    /// <returns>The new connection.</returns>
    public static SerialPortConnection FromMock(Type mockType) => new((ISerialPort)Activator.CreateInstance(mockType)!);

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
            Monitor.PulseAll(_queue);

            /* If there are any outstand requests notify the corresponding clients - callers of Execute. */
            if (pending != null)
                foreach (var request in pending)
                    request.Result.SetException(new OperationCanceledException());
        }
    }

    /// <summary>
    /// Add a command to be sent to the serial port to the queue.
    /// </summary>
    /// <param name="requests">The command to send to the device.</param>
    /// <returns>All lines sent from the device as a task.</returns>
    public Task<string[]>[] Execute(params SerialPortRequest[] requests)
    {
        /* Since we are expecting multi-threaded access lock the queue. */
        lock (_queue)
        {
            /* Queue is locked, we have exclusive access and can now safely add the entry. */
            Array.ForEach(requests, _queue.Enqueue);

            /* If queue executer thread is waiting (Monitor.Wait) for new entries wake it up for immediate processing the new entry. */
            Monitor.PulseAll(_queue);
        }

        /* Report the task related with the result promise. */
        return requests.Select(request => request.Result.Task).ToArray();
    }

    /// <summary>
    /// Executes a single command.
    /// </summary>
    /// <param name="request">Describes the request.</param>
    private void ExecuteCommand(SerialPortRequest request)
    {
        try
        {
            /* Send the command string to the device - <CR> is automatically added. */
            _port.WriteLine(request.Command);
        }
        catch (Exception e)
        {
            /* Unable to sent the command - report error to caller. */
            request.Result.SetException(e);

            return;
        }

        /* Collect response strings until expected termination string is detected. */
        for (var answer = new List<string>(); ;)
            try
            {
                /* Read a single response line from the device. */
                var reply = _port.ReadLine();

                /* If a device response ends with NAK there are invalid arguments. */
                if (reply.EndsWith("NAK"))
                {
                    request.Result.SetException(new ArgumentException(request.Command));

                    return;
                }

                /* Always remember the reply - even the terminating string. */
                answer.Add(reply);

                /* If the terminating string is detected the reply from the device is complete. */
                if (reply == request.End)
                {
                    /* Set the task result to all strings collected and therefore finish the task with success. */
                    request.Result.SetResult(answer.ToArray());

                    return;
                }
            }
            catch (Exception e)
            {
                /* 
                    If it is not possible to read something from the device report exception to caller. 
                    In case the device does not recognize the command it will not respond anything. Then
                    the read method call will throw a time exception as configured in the constructor.
                */
                request.Result.SetException(e);

                return;
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
            SerialPortRequest? request;

            /* Must have exclusive access to the queue to avoid data corruption. */
            lock (_queue)
            {
                /* Since we waited on the lock we should retest the running state. */
                if (!_running)
                    return;

                /* Try to get the first (oldest) entry from the queue. */
                if (!_queue.TryDequeue(out request))
                {
                    /* If queue is empty wait until someone intentionally wakes us up (Monitor.PulseAll) to avoid unnecessary processings. */
                    Monitor.Wait(_queue);

                    continue;
                }
            }

            /* Process the request - important: ExecuteCommand MUST NOT throw an exception. */
            ExecuteCommand(request);
        }
    }
}
