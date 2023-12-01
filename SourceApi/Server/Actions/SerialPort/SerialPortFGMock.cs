using SerialPortProxy;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public class SerialPortFGMock : ISerialPort
{
    private readonly Queue<QueueEntry> _replies = new();

    /// <inheritdoc/>
    public virtual void Dispose()
    {
    }

    /// <inheritdoc/>
    public virtual string ReadLine()
    {
        if (!_replies.TryDequeue(out var info))
            throw new TimeoutException("no reply in queue");

        return info.Reply;
    }

    /// <inheritdoc/>
    public virtual void WriteLine(string command)
    {
        switch (command)
        {
            case "TS":
                _replies.Enqueue("TSFG399   V703");
                break;
        }
    }
}
