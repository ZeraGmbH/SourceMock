using System.Text.RegularExpressions;
using MeterTestSystemApi.Models;
using MeterTestSystemApi.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using SourceApi.Model.Configuration;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApi.Actions.Probing.SerialPort;

/// <summary>
/// 
/// </summary>
public class CustomSerialPortFactory(IServiceProvider services, ILogger<CustomSerialPortFactory> logger) : ICustomSerialPortFactory
{
    private readonly Dictionary<string, ISerialPortConnection> _Connections = [];

    private readonly object _sync = new();

    private bool _initialized = false;

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
        {
            _initialized = true;

            foreach (var connection in _Connections.Values)
                connection.Dispose();

            Monitor.PulseAll(_sync);
        }
    }

    private ISerialPortConnection LookupPort(string name)
    {
        lock (_sync)
        {
            while (!_initialized)
                Monitor.Wait(_sync);

            return _Connections[name];
        }
    }

    /// <inheritdoc/>
    public void Initialize(Dictionary<string, CustomSerialPortConfiguration> ports)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("Custom serial ports already initialized");

            try
            {
                foreach (var customPort in ports)
                {
                    var config = customPort.Value?.SerialPort;

                    if (config == null) continue;

                    try
                    {
                        var log = services.GetRequiredService<ILogger<SerialPortConnection>>();

                        var port = config.ConfigurationType switch
                        {
                            SerialPortConfigurationTypes.Device => SerialPortConnection.FromSerialPort(config.Endpoint!, config.SerialPortOptions, log),
                            SerialPortConfigurationTypes.Network => SerialPortConnection.FromNetwork(config.Endpoint!, log),
                            _ => throw new NotSupportedException($"Unknown serial port configuration type {config.ConfigurationType}"),
                        };

                        // Remember
                        _Connections[customPort.Key] = port;
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical("Unable to configure custom serial port {Name}: {Exception}", customPort.Key, e.Message);
                    }
                }
            }
            finally
            {
                /* Use the new instance. */
                _initialized = true;

                /* Signal availability of meter test system. */
                Monitor.PulseAll(_sync);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<SerialPortReply[]> ExecuteAsync(string name, IEnumerable<SerialPortCommand> requests, IInterfaceLogger logger)
    {
        var commands =
            requests
                .Select(r =>
                    r.UseRegularExpression
                        ? SerialPortRequest.Create(r.Command, new Regex(r.Reply))
                        : SerialPortRequest.Create(r.Command, r.Reply))
                .ToArray();

        await Task.WhenAll(
            LookupPort(name)
                .CreateExecutor(InterfaceLogSourceTypes.SerialPort, name)
                .ExecuteAsync(logger, commands));

        return [.. commands.Select(SerialPortReply.FromRequest)];
    }
}