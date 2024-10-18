using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.Models.Logging;

namespace SerialPortProxy;

public partial class SerialPortConnection
{
    private class ExclusiveAccessItem<T>(Func<ISerialPort, IInterfaceConnection, T> algorithm, IInterfaceConnection logger) : QueueItem
    {
        private readonly TaskCompletionSource<T> _Task = new();

        private readonly Func<ISerialPort, IInterfaceConnection, T> _Algorithm = algorithm;

        private readonly IInterfaceConnection _Logger = logger;

        public Task<T> Task => _Task.Task;

        public override void Discard(SerialPortConnection connection)
        {
            connection._logger.LogWarning("Cancel exclusive port access");

            _Task.SetException(new OperationCanceledException());
        }

        public override void Execute(SerialPortConnection connection)
        {
            connection._logger.LogDebug("Starting exclusive port access");

            try
            {
                _Task.SetResult(_Algorithm(connection._port, _Logger));
            }
            catch (Exception e)
            {
                _Task.SetException(e);
            }
        }
    }

    private Task<T> Execute<T>(IInterfaceConnection connection, Func<ISerialPort, IInterfaceConnection, T> algorithm)
    {
        ArgumentNullException.ThrowIfNull(algorithm, nameof(algorithm));

        /* Since we are expecting multi-threaded access lock the queue. */
        lock (_queue)
        {
            /* Queue is locked, we have exclusive access and can now safely add the entry. */
            var item = new ExclusiveAccessItem<T>(algorithm, connection);

            _queue.Enqueue(item);

            /* If queue executer thread is waiting (Monitor.Wait) for new entries wake it up for immediate processing the new entry. */
            Monitor.Pulse(_queue);

            /* Report the task related with the result promise. */
            return item.Task;
        }
    }
}
