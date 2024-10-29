using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.Models.Logging;

namespace SerialPortProxy;

public partial class SerialPortConnection
{
    private class RequestBasedItem(SerialPortRequest[] requests, IInterfaceConnection logger) : QueueItem
    {
        private readonly SerialPortRequest[] _Requests = requests;

        private readonly IInterfaceConnection _Logger = logger;

        public override void Execute(SerialPortConnection connection)
        {
            /* Process the transaction until finished or some request failed - important: ExecuteCommand MUST NOT throw an exception. */
            var requests = _Requests;

            connection._logger.LogDebug("Starting transaction processing, commands: {RequestCount}", requests.Length);

            var failed = false;

            foreach (var request in requests)
                if (failed)
                    request.Result.SetException(new OperationCanceledException("previous command failed"));
                else
                    failed = !connection.ExecuteCommand(request, _Logger);
        }

        public override void Discard(SerialPortConnection connection)
        {
            foreach (var request in _Requests)
            {
                connection._logger.LogWarning("Cancel command {Command}", request.Command);

                request.Result.SetException(new OperationCanceledException());
            }
        }
    }

    private Task<string[]>[] ExecuteAsync(IInterfaceConnection connection, params SerialPortRequest[] requests)
    {
        ArgumentNullException.ThrowIfNull(requests, nameof(requests));

        /* Since we are expecting multi-threaded access lock the queue. */
        lock (_queue)
        {
            /* Queue is locked, we have exclusive access and can now safely add the entry. */
            if (requests.Length > 0)
                _queue.Enqueue(new RequestBasedItem(requests, connection));

            /* If queue executer thread is waiting (Monitor.Wait) for new entries wake it up for immediate processing the new entry. */
            Monitor.Pulse(_queue);
        }

        /* Report the task related with the result promise. */
        var tasks = new List<Task<string[]>>();

        foreach (var request in requests) tasks.Add(request.Result.Task);

        return [.. tasks];
    }

    /// <summary>
    /// Executes a single command.
    /// </summary>
    /// <param name="request">Describes the request.</param>
    /// <param name="connection"></param>
    /// <returns>Gesetzt, wenn die Ausführung erfolgreich war.</returns>
    private bool ExecuteCommand(SerialPortRequest request, IInterfaceConnection connection)
    {
        /* Prepare logging. */
        var requestId = Guid.NewGuid().ToString();

        IPreparedInterfaceLogEntry sendEntry = null!;
        Exception sendError = null!;

        try
        {
            _logger.LogDebug("Sending command {Command}", request.Command);

            /* Start logging. */
            sendEntry = connection.Prepare(new() { Outgoing = true, RequestId = requestId });

            /* Send the command string to the device - <CR> is automatically added. */
            _port.WriteLine(request.Command);

            _logger.LogDebug("Command {Command} accepted by device", request.Command);
        }
        catch (Exception e)
        {
            sendError = e;

            _logger.LogError("Command {Command} rejected: {Exception}", request.Command, e.Message);

            /* Unable to sent the command - report error to caller. */
            request.Result.SetException(e);
        }

        /* Finish logging. */
        if (sendEntry != null)
            try
            {
                sendEntry.Finish(new()
                {
                    Encoding = InterfaceLogPayloadEncodings.Raw,
                    Payload = request.Command,
                    PayloadType = "",
                    TransferException = sendError?.Message
                });
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to create log entry: {Exception}", e.Message);
            }

        /* Failed - we can stop now. */
        if (sendError != null) return false;

        /* Collect response strings until expected termination string is detected. */
        for (var answer = new List<string>(); ;)
        {
            IPreparedInterfaceLogEntry receiveEntry = null!;
            Exception receiveError = null!;
            var reply = "";

            try
            {
                /* Read a single response line from the device. */
                _logger.LogDebug("Wait for command {Command} reply", request.Command);

                /* Start logging. */
                receiveEntry = connection.Prepare(new() { Outgoing = false, RequestId = requestId });

                reply = ReadInput();

                _logger.LogDebug("Got reply {Reply} for command {Command}", reply, request.Command);

                /* If a device response ends with NAK there are invalid arguments. */
                if (reply.EndsWith("NAK"))
                {
                    _logger.LogError("Command {Command} reported NAK", request.Command);

                    throw new ArgumentException(request.Command);
                }

                /* Error handling for ERR commands. */
                if (reply.Contains("ER-") || reply.Contains("ERR-") || reply.Contains("ERROR"))
                {
                    _logger.LogError("Command {Command} reported ERROR {Reply}", request.Command, reply);

                    throw new ArgumentException(request.Command); ;
                }

                /* Always remember the reply - even the terminating string. */
                answer.Add(reply);

                /* If the terminating string is detected the reply from the device is complete. */
                if (request.Match(reply))
                {
                    _logger.LogDebug("Command {Command} finished, replies: {ReplyCount}", request.Command, answer.Count);

                    /* Set the task result to all strings collected and therefore finish the task with success. */
                    request.Result.SetResult([.. answer]);

                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Reading command {Command} reply failed: {Exception}", request.Command, e);

                /* 
                    If it is not possible to read something from the device report exception to caller. 
                    In case the device does not recognize the command it will not respond anything. Then
                    the read method call will throw a time exception as configured in the constructor.
                */
                receiveError = e;

                return false;
            }
            finally
            {
                if (receiveError != null) request.Result.SetException(receiveError);

                /* Finish logging. */
                if (receiveEntry != null)
                    try
                    {
                        receiveEntry.Finish(new()
                        {
                            Encoding = InterfaceLogPayloadEncodings.Raw,
                            Payload = reply,
                            PayloadType = "",
                            TransferException = receiveError?.Message
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unable to create log entry: {Exception}", e.Message);
                    }
            }
        }
    }
}
