using SerialPortProxy;

namespace RefMeterApiTests.PortMocks;

public class StandardPortMock : ISerialPort
{
    private static readonly string[] _replies = File.ReadAllLines(@"TestData/ameReply.txt");

    private readonly Queue<string> _queue = new();

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
            case "ATI01":
                _queue.Enqueue("ATIACK");

                break;
            case "AME":
                Array.ForEach(_replies, _queue.Enqueue);

                break;
        }
    }
}
