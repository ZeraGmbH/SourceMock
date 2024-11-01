using SerialPortProxy;
using SourceApi.Actions.SerialPort;

namespace BurdenApi.Actions;

/// <summary>
/// 
/// </summary>
public class BurdenSerialPortMock : ISerialPort
{
    /// <summary>
    /// Outgoing messages.
    /// </summary>
    private readonly Queue<QueueEntry> _replies = new();

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public byte? RawRead() => throw new NotSupportedException();

    /// <inheritdoc/>
    public void RawWrite(byte[] command) => throw new NotSupportedException();

    /// <inheritdoc/>
    public string ReadLine()
    {
        if (!_replies.TryDequeue(out var info))
        {
            Thread.Sleep(100);

            throw new TimeoutException("no reply in queue");
        }

        return info.Reply;
    }

    /// <inheritdoc/>
    public void WriteLine(string command)
    {
    }
}