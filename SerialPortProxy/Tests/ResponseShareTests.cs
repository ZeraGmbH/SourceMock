using SerialPortProxy;

namespace SerialPortProxyTests;

[TestFixture]
public class ResponseShareTests
{
    [Test, CancelAfter(2500)]
    public async Task Will_Share_Response_Async()
    {
        var count = 0;

        var cut = new ResponseShare<int, bool>((ctx) => Task.Delay(100).ContinueWith(_ => ++count, TaskScheduler.Default));

        var first = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => cut.ExecuteAsync(true)));

        Array.ForEach(first, v => Assert.That(v, Is.EqualTo(1)));

        for (; cut.IsBusy; Thread.Sleep(100))
            await TestContext.Out.WriteLineAsync("still busy - retry in 100ms");

        var second = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => cut.ExecuteAsync(true)));

        Array.ForEach(second, v => Assert.That(v, Is.EqualTo(2)));
    }
}
