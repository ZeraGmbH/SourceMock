using SerialPortProxy;

namespace SourceApi.Tests.Actions.Dosage.PortMocks;

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
        {
            Thread.Sleep(100);

            throw new TimeoutException("queue is empty");
        }

        /* For this special test make sure that there is some delay on execution. */
        Thread.Sleep(100);

        return reply;
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "ATI01":
                _queue.Enqueue("ATIACK");

                break;
            case "AME":
                _queue.Enqueue($"28;{_count++}");
                _queue.Enqueue("AMEACK");

                break;
        }
    }

    public void RawWrite(byte[] command) => throw new NotImplementedException();

    public byte? RawRead(int? timeout = null) => throw new NotImplementedException();
}
