using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using DutApi.Exceptions;
using DutApi.Models;
using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.Models.Logging;

namespace DutApi.Actions;

/// <summary>
/// Connect using SCPI.NET.
/// </summary>
/// <param name="logger">Logging helper.</param>
public class ScpiConnection(ILogger<ScpiConnection> logger) : IDeviceUnderTestConnection
{
    private static readonly Regex parseEndpoint = new(@"^(.+):([0-9]+)$");

    /// <summary>
    /// 
    /// </summary>
    public static readonly Regex ParseNumber = new(@"^([+-]?(\d+|(\d+\.\d*)|(\d*\.\d+))([Ee][+-]?\d+)?)$");

    private static readonly Regex parseNamedNumber = new(@"^.*:([^:]+);$");

    private static readonly HashSet<DutStatusRegisterTypes> _rawNumber = [DutStatusRegisterTypes.MeterConstant];

    private static readonly HashSet<DutStatusRegisterTypes> _raw = [
        DutStatusRegisterTypes.Serial,
        DutStatusRegisterTypes.CurrentRange,
        DutStatusRegisterTypes.CurrentRanges,
        DutStatusRegisterTypes.VoltageRange,
        DutStatusRegisterTypes.VoltageRanges,
        .._rawNumber
    ];

    private TcpClient _connection = null!;

    private Dictionary<DutStatusRegisterTypes, DutStatusRegisterInfo> _status = [];

    private InterfaceLogEntryConnection? _logConnection;

    /// <inheritdoc/>
    public void Dispose()
    {
        using (_connection)
            _connection = null!;
    }

    /// <inheritdoc/>
    public Task Initialize(string webSamId, DutConnection configuration, DutStatusRegisterInfo[] status)
    {
        /* Check for supported types. */
        if (configuration.Type != DutProtocolTypes.SCPIOverTCP) throw new NotImplementedException("only SCPI over TCP supported so far");

        /* Prepare logging. */
        _logConnection = new()
        {
            Endpoint = configuration.Endpoint,
            Protocol = InterfaceLogProtocolTypes.Tcp,
            WebSamType = InterfaceLogSourceTypes.DeviceUnderTest,
            WebSamId = webSamId
        };

        /* Test endpoint. */
        var match = parseEndpoint.Match(configuration.Endpoint ?? string.Empty) ?? throw new ArgumentException("invalid endpoint");

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

    private string ReadLine(CancellationToken? cancel)
    {
        var stream = _connection.GetStream();

        for (var buf = new byte[1024]; ;)
        {
            var sep = _collector.IndexOf(10);

            if (sep >= 0)
            {
                var line = Encoding.UTF8.GetString(_collector.Take(sep).ToArray());

                _collector.RemoveRange(0, sep + 1);

                return line;
            }

            var len = stream.Read(buf);

            if (len > 0)
                _collector.AddRange(buf.Take(len));

            cancel?.ThrowIfCancellationRequested();
        }
    }

    private void Flush()
    {
        _connection.GetStream().Flush();

        _collector.Clear();
    }

    /// <inheritdoc/>
    public Task<object[]> ReadStatusRegisters(IInterfaceLogger interfaceLogger, DutStatusRegisterTypes[] types, CancellationToken? cancel = null) => Task.Run(() =>
    {
        /* Prepare logging. */
        var connection = interfaceLogger.CreateConnection(_logConnection!);

        var result = new object[types.Length];

        for (var iRaw = 2; iRaw-- > 0;)
            lock (_collector)
            {
                var raw = iRaw == 1;

                /* Analyse input types. */
                var commands = types
                    .Select((DutStatusRegisterTypes type, int index) =>
                    {
                        /* See if type is used at all. */
                        if (!_status.TryGetValue(type, out var info)) info = null;

                        /* Create information structure. */
                        return new
                        {
                            Info = _raw.Contains(type) == raw ? info : null,
                            Index = index,
                            RawNumber = _rawNumber.Contains(type)
                        };
                    })
                    .Where(i => i.Info != null)
                    .ToList();

                if (commands.Count < 1) continue;

                /* Create a single command and send it to the device. */
                var summary = string.Join("|", commands.Select(i => $"{i.Info!.Address}?"));

                /* Prepare logging. */
                var requestId = Guid.NewGuid().ToString();
                var sendEntry = connection.Prepare(new() { Outgoing = true, RequestId = requestId });
                var sendInfo = new InterfaceLogPayload() { Encoding = InterfaceLogPayloadEncodings.Scpi, Payload = summary, PayloadType = "" };

                try
                {
                    Flush();

                    _connection.GetStream().Write(Encoding.UTF8.GetBytes($"{summary}\n"));

                }
                catch (Exception e)
                {
                    logger.LogError("Device under test at {Address} not available: {Exception}", _connection, e.Message);

                    sendInfo.TransferException = e.Message;

                    throw new DutIoException($"Device under test at {_connection} not available: {e.Message}", e);
                }
                finally
                {
                    sendEntry.Finish(sendInfo);
                }

                /* Prepare logging. */
                var receiveEntry = connection.Prepare(new() { Outgoing = false, RequestId = requestId });
                var receiveInfo = new InterfaceLogPayload() { Encoding = InterfaceLogPayloadEncodings.Scpi, Payload = "", PayloadType = "" };

                /* Read the raw values. */
                List<string> rawValues;

                try
                {
                    rawValues = commands.Select(c => ReadLine(cancel)).ToList();

                    receiveInfo.Payload = string.Join("\n", rawValues);
                }
                catch (Exception e)
                {
                    logger.LogError("Device under test did not respond to {Command} query: {Exception}", summary, e.Message);

                    receiveInfo.TransferException = e.Message;

                    throw new DutIoException($"Device under test did not respond to {summary} query: {e.Message}", e);
                }
                finally
                {
                    receiveEntry.Finish(receiveInfo);
                }

                /* In raw mode just copy over. */
                if (raw)
                    for (var i = 0; i < rawValues.Count; i++)
                    {
                        /* Load value and test for semantic. */
                        var rawValue = rawValues[i];
                        var command = commands[i];

                        if (command.RawNumber)
                        {
                            /* See if this is a number. */
                            var match = ParseNumber.Match(rawValue);

                            if (match?.Success != true) continue;

                            var value = double.Parse(match.Groups[1].Value);
                            var info = command.Info!;

                            if (info.Scale.GetValueOrDefault(0) != 0) value *= (double)info.Scale!;

                            result[command.Index] = value;
                        }
                        else
                            result[command.Index] = rawValue;
                    }
                else
                {
                    /* Create lookup map. */
                    foreach (var command in commands)
                    {
                        /* Lookup result. */
                        var info = command.Info!;
                        var prefix = string.Join(":", $"{info.Address}:".Split(":").Skip(1));
                        var rawValue = rawValues.FirstOrDefault(r => r.StartsWith(prefix));

                        if (rawValue == null) continue;

                        /* Check for number. */
                        var match = parseNamedNumber.Match(rawValue);

                        if (match?.Success != true) continue;

                        /* Get the value. */
                        var value = double.Parse(match.Groups[1].Value);

                        if (info.Scale.GetValueOrDefault(0) != 0) value *= (double)info.Scale!;

                        /* Remember it. */
                        result[command.Index] = value;
                    }
                }
            }

        return Task.FromResult(result);
    });

    /// <inheritdoc/>
    public Task<string[]> DirectCommand(IInterfaceLogger interfaceLogger, string command, int responseLines, CancellationToken? cancel = null) => Task.Run(() =>
    {
        /* Prepare logging. */
        var connection = interfaceLogger.CreateConnection(_logConnection!);

        /* Prepare logging. */
        var requestId = Guid.NewGuid().ToString();
        var sendEntry = connection.Prepare(new() { Outgoing = true, RequestId = requestId });
        var sendInfo = new InterfaceLogPayload() { Encoding = InterfaceLogPayloadEncodings.Scpi, Payload = command, PayloadType = "" };

        try
        {
            Flush();

            _connection.GetStream().Write(Encoding.UTF8.GetBytes($"{command}\n"));

            if (responseLines == 0)
            {
                sendInfo.Payload = $"{sendInfo.Payload}\n*OCP?";

                _connection.GetStream().Write(Encoding.UTF8.GetBytes("*OCP?\n"));

                responseLines = 1;
            }
        }
        catch (Exception e)
        {
            logger.LogError("Device under test at {Address} not available: {Exception}", _connection, e.Message);

            sendInfo.TransferException = e.Message;

            throw new DutIoException($"Device under test at {_connection} not available: {e.Message}", e);
        }
        finally
        {
            sendEntry.Finish(sendInfo);
        }

        /* Prepare logging. */
        var receiveEntry = connection.Prepare(new() { Outgoing = false, RequestId = requestId });
        var receiveInfo = new InterfaceLogPayload() { Encoding = InterfaceLogPayloadEncodings.Scpi, Payload = "", PayloadType = "" };

        /* Wait for response. */
        try
        {
            var rawValues = Enumerable.Range(0, responseLines).Select(i => ReadLine(cancel)).ToList();

            receiveInfo.Payload = string.Join("\n", rawValues);

            /* Caller must decode. */
            return Task.FromResult(rawValues.ToArray());
        }
        catch (Exception e)
        {
            logger.LogError("Device under test did not respond to {Command} query: {Exception}", command, e.Message);

            receiveInfo.TransferException = e.Message;

            throw new DutIoException($"Device under test did not respond to {command} query: {e.Message}", e);
        }
        finally
        {
            receiveEntry.Finish(receiveInfo);
        }
    });
}