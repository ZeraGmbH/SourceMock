using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;

namespace Tests;

class CounterMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    private readonly HashSet<int> _ids = new();

    public long Total = 0;

    public int[] ThreadIds
    {
        get
        {
            lock (_ids)
                return _ids.ToArray();
        }
    }

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        lock (_ids)
        {
            if (!_replies.TryDequeue(out var reply))
                throw new TimeoutException("no reply in quuue");

            _ids.Add(Thread.CurrentThread.ManagedThreadId);

            return reply;
        }
    }

    public void WriteLine(string command)
    {
        switch (command)
        {
            case "START":
                {

                    lock (_ids)
                    {
                        Total += 1;

                        _ids.Add(Thread.CurrentThread.ManagedThreadId);
                    }

                    _replies.Enqueue("STOP");

                    break;
                }
        }
    }
}


[TestFixture]
public class QueueTests
{
    private readonly NullLogger<SerialPortConnection> _logger = new();

    [Test]
    public async Task All_Commands_Are_Executed_On_The_Same_Thread()
    {
        var counter = new CounterMock();

        using var cut = SerialPortConnection.FromPortInstance(counter, _logger);

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(async () =>
            {
                for (var n = 20; n-- > 0;)
                {
                    Thread.Sleep(Random.Shared.Next(1, 2));

                    var requests = Enumerable.Range(0, Random.Shared.Next(1, 4)).Select(_ => SerialPortRequest.Create("START", "STOP")).ToArray();

                    await Task.WhenAll(cut.Execute(requests));
                }
            }
        )));

        Assert.That(counter.ThreadIds.Length, Is.EqualTo(1));
        Assert.That(counter.Total, Is.GreaterThanOrEqualTo(400).And.LessThanOrEqualTo(1600));

        Console.WriteLine($"Total={counter.Total}");
    }
}
