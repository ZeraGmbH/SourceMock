using System.Globalization;
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
    public TcpPortProxy(string serverAndPort)
    {
        /* Split the target. */
        var sep = serverAndPort.IndexOf(':');

        if (sep < 0)
            throw new ArgumentException(nameof(serverAndPort));

        var server = serverAndPort[..sep];
        var port = ushort.Parse(serverAndPort[(sep + 1)..], CultureInfo.InvariantCulture);

        /* Create the connection. */
        _client = new TcpClient(server, port);
        _stream = _client.GetStream();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _stream.Dispose();
        _client.Dispose();
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
    public void WriteLine(string command)
    {
        /* Append the line separator and convert string to bytes. */
        var bytes = Encoding.ASCII.GetBytes($"{command}\r");

        /* Send to connection. */
        _stream.Write(bytes, 0, bytes.Length);
    }
}