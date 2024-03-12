using System.Globalization;
using System.Text.RegularExpressions;
using DutApi.Exceptions;
using DutApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScpiNet;

namespace DutApi.Actions;

/// <summary>
/// Connect using SCPI.NET.
/// </summary>
/// <param name="services">Dependency injection.</param>
/// <param name="logger">Logging helper.</param>
public class ScpiConnection(IServiceProvider services, ILogger<ScpiConnection> logger) : IDeviceUnderTestConnection
{
    class Device(IScpiConnection connection, string deviceId, ILogger<ScpiDevice> logger) : ScpiDevice(connection, deviceId, logger)
    {
        public Task<string> Execute(string command) => Query(command);
    }

    private static readonly Regex parseEndpoint = new(@"^(.+):([0-9]+)$");

    private static readonly Regex parseNumber = new(@"^.*:([^:]+);$");

    private static readonly HashSet<DutStatusRegisterTypes> _raw = [DutStatusRegisterTypes.Serial];

    private IScpiConnection _connection = null!;

    private Dictionary<DutStatusRegisterTypes, DutStatusRegisterInfo> _status = [];

    /// <inheritdoc/>
    public void Dispose()
    {
        using (_connection)
            _connection = null!;
    }

    /// <inheritdoc/>
    public async Task Initialize(DutConnection configuration, DutStatusRegisterInfo[] status)
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

        /* Create SCPI connection - must use appropriate timeout due to integration times. */
        _connection = new TcpScpiConnection(
            match.Groups[1].Value,
            ushort.Parse(match.Groups[2].Value),
            5000,
            services.GetRequiredService<ILogger<TcpScpiConnection>>()
        );

        /* Open the physical connection. */
        await _connection.Open();
    }

    /// <inheritdoc/>
    public async Task<object[]> ReadStatusRegisters(DutStatusRegisterTypes[] types)
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
            await _connection.WriteString(summary);
        }
        catch (InvalidOperationException e)
        {
            logger.LogError("Device under test at {Address} not connected: {Exception}", _connection.DevicePath, e.Message);

            throw new DutIoException($"Device under test at {_connection.DevicePath} not available: {e.Message}", e);
        }

        /* Process all replies. */
        try
        {
            /* Read the raw values. */
            var rawValues = (await _connection.ReadString()).Split("\n");

            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];

                /* Get the raw values. */
                var rawValue = rawValues[i];

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
        }
        catch (TimeoutException e)
        {
            logger.LogError("Device under test did not respond to {Command} query: {Exception}", summary, e.Message);

            throw new DutIoException($"Device under test did not respond to {summary} query: {e.Message}", e);
        }


        return result;
    }
}