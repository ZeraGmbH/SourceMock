using SerialPortProxy;

namespace SourceApi.Tests.Actions.Dosage.PortMocks;

public class CommandPeekMock : ISerialPort
{
    private readonly Queue<string> _queue = new();

    private readonly string[] _replies;

    public readonly List<string> Commands = new();

    public CommandPeekMock(params string[] replies)
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
        Commands.Add(command);

        Array.ForEach(_replies, _queue.Enqueue);
    }
}
