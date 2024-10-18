using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.SerialPort.FG30x;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Model.Configuration;

namespace SourceApi.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
/// <param name="services"></param>
/// <param name="logger"></param>
public class SerialPortConnectionFactory(IServiceProvider services, ILogger<SerialPortConnectionFactory> logger) : ISerialPortConnectionFactory, IDisposable
{
    class NullConnection : ISerialPortConnection
    {
        class Executor : ISerialPortConnectionExecutor
        {
            public Task<string[]>[] Execute(IInterfaceLogger logger, params SerialPortRequest[] requests) =>
                requests.Select(r => Task.FromException<string[]>(new NotSupportedException())).ToArray();

            public Task<T> RawExecute<T>(IInterfaceLogger logger, Func<ISerialPort, IInterfaceConnection, T> algorithm)
                => Task.FromException<T>(new NotSupportedException());
        }

        public void Dispose()
        {
        }


        public void RegisterEvent(Regex pattern, Action<Match> handler)
        {
        }

        public ISerialPortConnectionExecutor CreateExecutor(InterfaceLogSourceTypes type, string id) => new Executor();
    }

    private readonly object _sync = new();

    private bool _initialized = false;

    private ISerialPortConnection? _connection;

    /// <inheritdoc/>
    public ISerialPortConnection Connection
    {
        get
        {
            lock (_sync)
                while (!_initialized)
                    Monitor.Wait(_sync);

            return _connection!;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_sync)
            _initialized = true;
    }

    /// <inheritdoc/>
    public void Initialize(MeterTestSystemTypes? type, SerialPortConfiguration? settings)
    {
        lock (_sync)
        {
            if (_initialized) throw new InvalidOperationException("serial port connection already configured");

            try
            {
                if (settings != null && type.HasValue)
                    if (type == MeterTestSystemTypes.MT786 || type == MeterTestSystemTypes.FG30x)
                        switch (settings.ConfigurationType)
                        {
                            case SerialPortConfigurationTypes.Mock:
                                switch (type)
                                {
                                    case MeterTestSystemTypes.FG30x:
                                        _connection = SerialPortConnection.FromMock<SerialPortFGMock>(services.GetRequiredService<ILogger<ISerialPortConnection>>());
                                        break;
                                    case MeterTestSystemTypes.MT786:
                                        _connection = SerialPortConnection.FromMock<SerialPortMTMock>(services.GetRequiredService<ILogger<ISerialPortConnection>>());
                                        break;
                                }
                                break;
                            case SerialPortConfigurationTypes.Network:
                                _connection = SerialPortConnection.FromNetwork(settings.Endpoint!, services.GetRequiredService<ILogger<ISerialPortConnection>>());
                                break;
                            case SerialPortConfigurationTypes.Device:
                                _connection = SerialPortConnection.FromSerialPort(settings.Endpoint!, services.GetRequiredService<ILogger<ISerialPortConnection>>());
                                break;
                        }
            }
            catch (Exception e)
            {
                logger.LogCritical("Unable to create serial port connection: {Exception}", e.Message);
            }
            finally
            {
                _connection ??= new NullConnection();

                _initialized = true;

                Monitor.PulseAll(_sync);
            }
        }
    }
}