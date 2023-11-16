using Microsoft.Extensions.Logging;

using WebSamDeviceApis.Actions.SerialPort;

namespace WebSamDeviceApis.Tests.Actions.Dosage;

public class DeviceLogger : ILogger<SerialPortSource>
{
    class Scope : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => new Scope();

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        throw new ArgumentException(formatter(state, exception));
}
