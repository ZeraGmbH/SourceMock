using SerialPortProxy;

namespace RefMeterApiTests.PortMocks;

public class CountingMock : ISerialPort
{
    private readonly Queue<string> _queue = new();

    private int _count = 50;

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (!_queue.TryDequeue(out var reply))
            throw new TimeoutException("queue is empty");

        /* For this special test make sure that there is some delay on execution. */
        Thread.Sleep(100);

        return reply;
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "AME":
                _queue.Enqueue($"28;{_count++}");
                _queue.Enqueue("AMEACK");

                break;
        }
    }
}
