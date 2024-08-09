using BarcodeApi.Actions.Device;
using Microsoft.Extensions.Logging;

namespace BarcodeApiTests;

[TestFixture]
public class BarcodeReaderTests
{
    class Logger : ILogger<BarcodeReader>
    {
        public readonly List<string> Messages = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception));
    }

    private static Logger _logger = new Logger();

    [Ignore("requires barcode input")]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Can_Reopen_After_Dispose(bool waitForBarcode)
    {
        var path = Environment.GetEnvironmentVariable("WEBSAM_BarcodeReader__Device");

        if (string.IsNullOrEmpty(path)) return;

        for (var i = 0; i++ < 4;)
        {
            var code = new TaskCompletionSource<string>();
            var task = code.Task;

            using (var reader = new BarcodeReader(path, _logger))
            {
                if (waitForBarcode)
                    reader.BarcodeReceived += code.SetResult;
                else
                {
                    await Task.Delay(100);

                    code.SetResult("simulation");
                }

                await task;
            }

            Assert.That(task.Result, Is.Not.Empty);
        }

        TestContext.Out.WriteLine(string.Join("\n", _logger.Messages));
    }
}
