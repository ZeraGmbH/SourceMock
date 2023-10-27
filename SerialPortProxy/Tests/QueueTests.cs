using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NuGet.Frameworks;
using NUnit.Framework.Internal;
using SerialPortProxy;

namespace Tests;


/// <summary>
/// Mock replying to a specific command only and remebering number of calls 
/// and thread of execution.
/// </summary>
class CounterMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    /// <summary>
    /// All thread identifiers seen while executing commands.
    /// </summary>
    private readonly HashSet<int> _ids = new();

    /// <summary>
    /// Total number of commands seen.
    /// </summary>
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
                throw new TimeoutException("no reply in queue");

            /* Remember the thread reading a reply. */
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
                        /* Count each command. */
                        Total += 1;

                        /* Remember the thread sending a command. */
                        _ids.Add(Thread.CurrentThread.ManagedThreadId);
                    }

                    _replies.Enqueue("STOP");

                    break;
                }
        }
    }
}

/// <summary>
/// A mock executing commands starting with R.
/// </summary>
class GroupMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public string ReadLine()
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        throw new TimeoutException("no reply in quuue");
    }

    public void WriteLine(string command)
    {
        if (command.StartsWith("R"))
            _replies.Enqueue(command);
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

        /* Start 20 threads in parallel sending 20 command groups with 1 to 4 commands each. */
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(async () =>
            {
                for (var n = 20; n-- > 0;)
                    /* Number of requests in group varies between 1 and 4 (both inclusive). */
                    await Task.WhenAll(cut.Execute(Enumerable.Range(0, Random.Shared.Next(1, 5)).Select(_ => SerialPortRequest.Create("START", "STOP")).ToArray()));
            }
        )));

        /* All commands must be served from a single thread. */
        Assert.That(counter.ThreadIds.Length, Is.EqualTo(1));

        /* There must be at least 400 (20*20*1) individual commands and at most 1600 (20*20*4). */
        Assert.That(counter.Total, Is.GreaterThanOrEqualTo(400).And.LessThanOrEqualTo(1600));

        /* Just as a visual control. */
        TestContext.WriteLine($"Total={counter.Total}");
    }

    [Test]
    public async Task Failure_In_Request_Group_Terminates_Group()
    {
        var groups = new GroupMock();

        using var cut = SerialPortConnection.FromPortInstance(groups, _logger);

        await Task.WhenAll(cut.Execute(SerialPortRequest.Create("R1", "R1")));

        var tasks = cut.Execute(
            /* Will process. */
            SerialPortRequest.Create("R2", "R2"),
            /* Will be discarded and therefore generate a TimeoutException. */
            SerialPortRequest.Create("xR3", "R3"),
            /* Could process but will be aborted with a OperationCanceledException due to the previous failure. */
            SerialPortRequest.Create("R4", "R4"));

        /* Just add another valid command. */
        await cut.Execute(SerialPortRequest.Create("R9", "R9"))[0];

        await tasks[0];

        /* May block if no await on the R9 command before - possibly due to the sync-over-async way ThrowAsync works. */
        Assert.ThrowsAsync<TimeoutException>(async () => await tasks[1]);
        Assert.ThrowsAsync<OperationCanceledException>(async () => await tasks[2]);
    }
}
