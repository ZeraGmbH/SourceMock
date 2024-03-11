using System.Globalization;
using System.Text.RegularExpressions;
using DutApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScpiNet;

namespace DutApi.Actions;

/// <summary>
/// Connect using SCPI.NET.
/// </summary>
/// <param name="services">Dependency injection.</param>
public class ScpiConnection(IServiceProvider services) : IDeviceUnderTestConnection
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

        for (var i = 0; i < types.Length; i++)
        {
            /* See if register is configured. */
            var type = types[i];

            if (!_status.TryGetValue(type, out var info)) continue;

            /* Read the raw value. */
            await _connection.WriteString($"{info.Address}?");

            var rawValue = await _connection.ReadString();

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

        return result;
    }
}