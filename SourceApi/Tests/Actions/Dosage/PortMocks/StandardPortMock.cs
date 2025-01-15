using SerialPortProxy;

namespace SourceApi.Tests.Actions.Dosage.PortMocks;

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

        Thread.Sleep(100);

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

    public void RawWrite(byte[] command) => throw new NotImplementedException();

    public byte? RawRead(int? timeout = null) => throw new NotImplementedException();
}
