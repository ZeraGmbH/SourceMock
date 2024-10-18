using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using SourceApi.Model.Configuration;
using ZERA.WebSam.Shared.Models.Logging;
using ZIFApi.Models;

namespace ZIFApi.Actions;

/// <summary>
/// 
/// </summary>
public class ZIFDevicesFactory(IServiceProvider services, ILogger<ZIFDevicesFactory> logger) : IZIFDevicesFactory
{
    private class ZIFDevice(ISerialPortConnection port, IZIFProtocol protocol) : IZIFDevice
    {
        private readonly ISerialPortConnection Port = port;

        private readonly IZIFProtocol Protocol = protocol;

        public Task<ZIFVersionInfo> GetVersion(IInterfaceLogger logger)
            => Protocol.GetVersion(Port, logger);

        public void Terminate() => Port.Dispose();
    }

    private readonly object _sync = new();

    private bool _initialized = false;

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
        {
            _initialized = true;

            foreach (var device in _Devices)
                device?.Terminate();

            Monitor.PulseAll(_sync);
        }
    }

    private readonly List<ZIFDevice?> _Devices = [];

    /// <inheritdoc/>
    public IZIFDevice[] Devices
    {
        get
        {
            lock (_sync)
                return [.. _Devices];
        }
    }

    /// <inheritdoc/>
    public void Initialize(List<ZIFConfiguration> sockets)
    {
        lock (_sync)
        {
            /* Many not be created more than once, */
            if (_initialized) throw new InvalidOperationException("Meter test system already initialized");

            try
            {
                for (var i = 0; i < sockets.Count; i++)
                {
                    var socket = sockets[i];

                    if (socket?.Type == null || string.IsNullOrEmpty(socket.SerialPort?.Endpoint))
                        _Devices.Add(null);
                    else
                        try
                        {
                            var config = socket.SerialPort!;

                            var log = services.GetRequiredService<ILogger<SerialPortConnection>>();

                            var protocol = services.GetRequiredKeyedService<IZIFProtocol>(socket.Type);

                            var port = config.ConfigurationType switch
                            {
                                SerialPortConfigurationTypes.Device => SerialPortConnection.FromSerialPort(config.Endpoint, config.SerialPortOptions, log, false),
                                SerialPortConfigurationTypes.Network => SerialPortConnection.FromNetwork(config.Endpoint, log, false),
                                _ => throw new NotSupportedException($"Unknown serial port configuration type {config.ConfigurationType}"),
                            };

                            // Remember
                            _Devices.Add(new(port, protocol));
                        }
                        catch (Exception e)
                        {
                            logger.LogCritical("Unable to configure ZIF socket {Index} {Exception}", i + 1, e.Message);
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
}