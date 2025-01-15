using System.Net.Sockets;
using System.Text;

namespace SerialPortProxy;

/// <summary>
/// Connect to a serial port using a TCP passthrough.
/// </summary>
public class TcpPortProxy : ISerialPort
{
    private readonly TcpClient _client;

    private readonly NetworkStream _stream;

    private readonly byte[] _readBuf = new byte[1000];

    private readonly List<byte> _collector = new();

    /// <summary>
    /// Initialize the new connection.
    /// </summary>
    /// <param name="serverAndPort">Server name or IP and TCP port to use -
    /// separated by a colon.</param>
    /// <param name="readTimeout">Maximum time to wait on new data (in milliseconds).</param>
    /// <param name="writeTimeout">Maximum time for a write operation to finish (in milliseconds).</param>
    public TcpPortProxy(string serverAndPort, int? readTimeout = null, int? writeTimeout = null)
    {
        /* Split the target. */
        var sep = serverAndPort.IndexOf(':');

        if (sep < 0)
            throw new ArgumentException(nameof(serverAndPort));

        var server = serverAndPort[..sep];
        var port = ushort.Parse(serverAndPort[(sep + 1)..]);

        /* Create the connection. */
        _client = new TcpClient(AddressFamily.InterNetwork)
        {
            SendTimeout = writeTimeout ?? 30000,
            ReceiveTimeout = readTimeout ?? 30000,
        };

        try
        {
            _client.Connect(server, port);

            /* Attach to the stream. */
            _stream = _client.GetStream();
        }
        catch (System.Exception)
        {
            /* Proper cleanup. */
            Dispose();

            throw;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }

    /// <inheritdoc/>
    public string ReadLine()
    {
        for (; ; )
        {
            /* See if there is some complete line ready to be dispatched. */
            var eol = _collector.FindIndex(b => b == 13);

            if (eol >= 0)
            {
                /* Extract the line and remove from collector cache. */
                var str = Encoding.ASCII.GetString(_collector.Take(eol).ToArray());

                _collector.RemoveRange(0, eol + 1);

                return str;
            }

            /* Ask connection for more data. */
            var len = _stream.Read(_readBuf, 0, _readBuf.Length);

            if (len <= 0)
                throw new EndOfStreamException();

            /* Just add to collector to check for complete lines. */
            _collector.AddRange(_readBuf.Take(len));
        }
    }

    /// <inheritdoc/>
    public void WriteLine(string command) => RawWrite(Encoding.ASCII.GetBytes($"{command}\r"));

    /// <inheritdoc/>
    public byte? RawRead(int? timeout = null)
    {
        if (_collector.Count < 1)
        {
            /* Busy wait while no data is available. */
            if (timeout != null)
                for (var end = DateTime.UtcNow.AddMilliseconds(timeout.Value); !_stream.DataAvailable; Thread.Sleep(10))
                    if (DateTime.UtcNow >= end)
                        throw new TimeoutException("read operation timed out");

            var data = _stream.ReadByte();

            return data == -1 ? null : checked((byte)data);
        }

        var next = _collector[0];

        _collector.RemoveAt(0);

        return next;
    }

    /// <inheritdoc/>
    public void RawWrite(byte[] command) => _stream.Write(command, 0, command.Length);
}