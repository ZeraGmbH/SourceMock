using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework.Internal;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

namespace Tests;

/// <summary>
/// A mock executing commands starting with R.
/// </summary>
class GroupMock : ISerialPort
{
    private readonly Queue<string> _replies = new();

    public void Dispose()
    {
    }

    public string ReadLine(CancellationToken? cancel)
    {
        if (_replies.TryDequeue(out var reply))
            return reply;

        Thread.Sleep(100);

        throw new TimeoutException("no reply in quuue");
    }

    public void WriteLine(string command, CancellationToken? cancel)
    {
        if (command.StartsWith("R"))
            _replies.Enqueue(command);
    }

    public void RawWrite(byte[] command, CancellationToken? cancel) => throw new NotImplementedException();

    public byte? RawRead(CancellationToken? cancel) => throw new NotImplementedException();
}

[TestFixture]
public class QueueTests
{
    static QueueTests()
    {
        SerialPortConnection.ActivateUnitTestMode(2000);
    }

    private readonly NullLogger<ISerialPortConnection> _logger = new();

    [Test]
    public async Task Failure_In_Request_Group_Terminates_Group_Async()
    {
        var groups = new GroupMock();

        using var cut = SerialPortConnection.FromMockedPortInstance(groups, _logger);

        await Task.WhenAll(cut.CreateExecutor(InterfaceLogSourceTypes.Source).ExecuteAsync(new NoopInterfaceLogger(), CancellationToken.None, SerialPortRequest.Create("R1", "R1")));

        var tasks = cut.CreateExecutor(InterfaceLogSourceTypes.Source).ExecuteAsync(
            new NoopInterfaceLogger(),
            CancellationToken.None,
            /* Will process. */
            SerialPortRequest.Create("R2", "R2"),
            /* Will be discarded and therefore generate a TimeoutException. */
            SerialPortRequest.Create("xR3", "R3"),
            /* Could process but will be aborted with a OperationCanceledException due to the previous failure. */
            SerialPortRequest.Create("R4", "R4"));

        /* Just add another valid command. */
        await cut.CreateExecutor(InterfaceLogSourceTypes.Source).ExecuteAsync(new NoopInterfaceLogger(), CancellationToken.None, SerialPortRequest.Create("R9", "R9"))[0];

        await tasks[0];

        /* May block if no await on the R9 command before - possibly due to the sync-over-async way ThrowAsync works. */
        Assert.ThrowsAsync<TimeoutException>(async () => await tasks[1]);
        Assert.ThrowsAsync<OperationCanceledException>(async () => await tasks[2]);
    }
}
