using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace SerialPortProxy;

public partial class SerialPortConnection
{
    private class ExclusiveAccessItem<T>(Func<ISerialPort, IInterfaceConnection, ICancellationService?, T> algorithm, ICancellationService? cancel, IInterfaceConnection logger) : QueueItem
    {
        private readonly TaskCompletionSource<T> _Task = new();

        private readonly Func<ISerialPort, IInterfaceConnection, ICancellationService?, T> _Algorithm = algorithm;

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
                _Task.SetResult(_Algorithm(connection._port, _Logger, cancel));
            }
            catch (Exception e)
            {
                _Task.SetException(e);
            }
        }
    }

    private Task<T> ExecuteAsync<T>(IInterfaceConnection connection, ICancellationService? cancel, Func<ISerialPort, IInterfaceConnection, ICancellationService?, T> algorithm)
    {
        ArgumentNullException.ThrowIfNull(algorithm, nameof(algorithm));

        /* Since we are expecting multi-threaded access lock the queue. */
        lock (_queue)
        {
            /* Queue is locked, we have exclusive access and can now safely add the entry. */
            var item = new ExclusiveAccessItem<T>(algorithm, cancel, connection);

            _queue.Enqueue(item);

            /* If queue executer thread is waiting (Monitor.Wait) for new entries wake it up for immediate processing the new entry. */
            Monitor.Pulse(_queue);

            /* Report the task related with the result promise. */
            return item.Task;
        }
    }
}
