using SerialPortProxy;

namespace RefMeterApiTests.PortMocks;

public class FixedReplyMock : ISerialPort
{
    private readonly Queue<string> _queue = new();

    private readonly string[] _replies;

    public FixedReplyMock(params string[] replies)
    {
        _replies = replies;
    }

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (_queue.TryDequeue(out var reply))
            return reply;

        throw new TimeoutException("queue is empty");
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "AME":
                Array.ForEach(this._replies, _queue.Enqueue);

                break;
        }
    }
}
