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
            var result = new object[types.Length];

            /* Analyse input types. */
            var commands = types
                .Select((DutStatusRegisterTypes type, int index) =>
                {
                    if (!_status.TryGetValue(type, out var info)) info = null;

                    return new { Raw = _raw.Contains(type), Info = info, Index = index };
                })
                .Where(i => i.Info != null)
                .ToList();

            /* Create a single command and send it to the device. */
            var summary = string.Join("|", commands.Select(i => $"{i.Info!.Address}?"));

            try
            {
                Flush();

                _connection.GetStream().Write(Encoding.UTF8.GetBytes($"{summary}\n"));
            }
            catch (Exception e)
            {
                logger.LogError("Device under test at {Address} not available: {Exception}", _connection, e.Message);

                throw new DutIoException($"Device under test at {_connection} not available: {e.Message}", e);
            }

            /* Process all replies. */
            foreach (var command in commands)
                try
                {
                    /* Read the raw values. */
                    var rawValue = ReadLine();

                    /* Convert the value. */
                    if (command.Raw)
                        result[command.Index] = rawValue;
                    else
                    {
                        /* Check for number. */
                        var match = parseNumber.Match(rawValue);

                        if (match == null) continue;

                        /* Get the value. */
                        var value = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                        /* Eventually scale it. */
                        if (command.Info!.Scale.GetValueOrDefault(0) != 0) value *= (double)command.Info.Scale!;

                        /* Remember it. */
                        result[command.Index] = value;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("Device under test did not respond to {Command} query: {Exception}", summary, e.Message);

                    throw new DutIoException($"Device under test did not respond to {summary} query: {e.Message}", e);
                }

            return Task.FromResult(result);
        }
    });
}