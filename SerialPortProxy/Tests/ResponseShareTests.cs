using System.Globalization;
using SerialPortProxy;

namespace SerialPortProxyTests;

[TestFixture]
public class ResponseShareTests
{
    [Test, Timeout(1500)]
    public async Task Will_Shared_Response()
    {
        var count = 0;

        var cut = new ResponseShare<int>(() => Task.Delay(100).ContinueWith(_ => ++count));

        var first = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => cut.Execute()));

        Array.ForEach(first, v => Assert.That(v, Is.EqualTo(1)));

        for (; cut.IsBusy; Thread.Sleep(100))
            TestContext.WriteLine("still busy - retry in 100ms");

        var second = await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => cut.Execute()));

        Array.ForEach(second, v => Assert.That(v, Is.EqualTo(2)));
    }
}
