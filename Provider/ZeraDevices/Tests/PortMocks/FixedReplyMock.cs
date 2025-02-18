using SerialPortProxy;

namespace ZeraDeviceTests.PortMocks;

public class FixedReplyMock(params string[] replies) : ISerialPort
{
    private readonly Queue<string> _queue = new();

    private readonly string[] _replies = replies;

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (_queue.TryDequeue(out var reply))
            return reply;

        Thread.Sleep(100);

        throw new TimeoutException("queue is empty");
    }

    public void WriteLine(string command) =>
        Array.ForEach(_replies, _queue.Enqueue);

    public void RawWrite(byte[] command) => throw new NotImplementedException();

    public byte? RawRead(int? timeout = null) => throw new NotImplementedException();
}
