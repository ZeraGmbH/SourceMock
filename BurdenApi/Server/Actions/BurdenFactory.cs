using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using SourceApi.Model.Configuration;

namespace BurdenApi.Actions;

/// <summary>
/// 
/// </summary>
public class BurdenFactory(IServiceProvider services, ILogger<BurdenFactory> logger) : IBurdenFactory
{
#pragma warning disable CS9113 // Parameter is unread.
    private class FallbackBurden(ISerialPortConnection port) : IBurden { }
#pragma warning restore CS9113 // Parameter is unread.

    private ISerialPortConnection _Connection = null!;

    private readonly object _sync = new();

    private bool _initialized = false;

    /// <inheritdoc />
    public IBurden Burden
    {
        get
        {
            lock (_sync)
            {
                while (!_initialized)
                    Monitor.Wait(_sync);

                return _Connection != null ? new FallbackBurden(_Connection) : null!;
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
        {
            _initialized = true;

            _Connection.Dispose();

            Monitor.PulseAll(_sync);
        }
    }

    /// <inheritdoc/>
    public void Initialize(BurdenConfiguration? configuration)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("Burden already initialized");

            try
            {
                var config = configuration?.SerialPort;

                if (config == null) return;

                try
                {
                    var log = services.GetRequiredService<ILogger<SerialPortConnection>>();

                    _Connection = config.ConfigurationType switch
                    {
                        SerialPortConfigurationTypes.Device => SerialPortConnection.FromSerialPort(config.Endpoint!, config.SerialPortOptions, log),
                        SerialPortConfigurationTypes.Network => SerialPortConnection.FromNetwork(config.Endpoint!, log),
                        SerialPortConfigurationTypes.Mock => SerialPortConnection.FromMock<BurdenSerialPortMock>(log),
                        _ => throw new NotSupportedException($"Unknown serial port configuration type {config.ConfigurationType}"),
                    };
                }
                catch (Exception e)
                {
                    logger.LogCritical("Unable to configure burden: {Exception}", e.Message);
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
}