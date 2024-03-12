using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using DutApi.Exceptions;
using DutApi.Models;
using Microsoft.Extensions.Logging;

namespace DutApi.Actions;

/// <summary>
/// Connect using SCPI.NET.
/// </summary>
/// <param name="logger">Logging helper.</param>
public class ScpiConnection(ILogger<ScpiConnection> logger) : IDeviceUnderTestConnection
{
    private static readonly Regex parseEndpoint = new(@"^(.+):([0-9]+)$");

    private static readonly Regex parseNumber = new(@"^.*:([^:]+);$");

    private static readonly HashSet<DutStatusRegisterTypes> _raw = [DutStatusRegisterTypes.Serial];

    private TcpClient _connection = null!;

    private Dictionary<DutStatusRegisterTypes, DutStatusRegisterInfo> _status = [];

    /// <inheritdoc/>
    public void Dispose()
    {
        using (_connection)
            _connection = null!;
    }

    /// <inheritdoc/>
    public Task Initialize(DutConnection configuration, DutStatusRegisterInfo[] status)
    {
        /* Check for supported types. */
        if (configuration.Type != DutProtocolTypes.SCPIOverTCP) throw new NotImplementedException("only SCPI over TCP supported so far");

        /* Test endpoint. */
        var match = parseEndpoint.Match(configuration.Endpoint ?? string.Empty);

        if (match == null) throw new ArgumentException("invalid endpoint");

        /* Load status registers. */
        _status = status
            .Where(s => !string.IsNullOrEmpty(s.Address))
            .ToDictionary(s => s.Type);

        try
        {
            /* Create SCPI connection - must use appropriate timeout due to integration times. */
            _connection =
                new(match.Groups[1].Value, ushort.Parse(match.Groups[2].Value))
                {
                    SendTimeout = 10000,
                    ReceiveTimeout = 5000
                };
        }
        catch (Exception e)
        {
            logger.LogError("Device under test at {Address} not available: {Exception}", _connection, e.Message);

            throw new DutIoException($"Device under test at {_connection} not available: {e.Message}", e);
        }

        return Task.CompletedTask;
    }

    private readonly List<byte> _collector = new();

    private string ReadLine()
    {
        var stream = _connection.GetStream();

        for (; ; )
        {
            var sep = _collector.IndexOf(10);

            if (sep >= 0)
            {
                var line = Encoding.UTF8.GetString(_collector.Take(sep).ToArray());

                _collector.RemoveRange(0, sep + 1);

                return line;
            }

            var buf = new byte[1024];
            var len = stream.Read(buf);

            if (len > 0)
                _collector.AddRange(buf.Take(len));
        }
    }

    private void Flush()
    {
        _connection.GetStream().Flush();

        _collector.Clear();
    }

    /// <inheritdoc/>
    public Task<object[]> ReadStatusRegisters(DutStatusRegisterTypes[] types) => Task.Run(() =>
    {
        lock (_collector)
        {
            try
            {
                /* Cleanup junk. */
                Flush();
            }
            catch (Exception e)
            {
                logger.LogError("Device under test at {Address} not available: {Exception}", _connection, e.Message);

                throw new DutIoException($"Device under test at {_connection} not available: {e.Message}", e);
            }

            /* Process all replies. */
            var result = new object[types.Length];
            var stream = _connection.GetStream();

            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];

                /* See if type is supported. */
                if (_status.TryGetValue(type, out var info))
                    try
                    {
                        /* Send the request. */
                        stream.Write(Encoding.UTF8.GetBytes($"{info.Address}?\n"));

                        /* Read the raw values. */
                        var rawValue = ReadLine();

                        /* Convert the value. */
                        if (_raw.Contains(type))
                            result[i] = rawValue;
                        else
                        {
                            /* Check for number. */
                            var match = parseNumber.Match(rawValue);

                            if (match == null) continue;

                            /* Get the value. */
                            var value = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                            /* Eventually scale it. */
                            if (info.Scale.GetValueOrDefault(0) != 0) value *= (double)info.Scale!;

                            /* Remember it. */
                            result[i] = value;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Device under test did not respond to {Command} query: {Exception}", info.Address, e.Message);

                        throw new DutIoException($"Device under test did not respond to {info.Address} query: {e.Message}", e);
                    }
            }

            return Task.FromResult(result);
        }
    });
}