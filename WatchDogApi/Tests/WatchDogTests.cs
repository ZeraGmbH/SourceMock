using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using WatchDogApi.Actions;
using Microsoft.Extensions.DependencyInjection;
using WatchDogApi.Models;
using WatchDogApi.Services;
using System.Threading.Tasks;
using Moq;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Actions;
using ZstdSharp.Unsafe;

namespace WatchDogApiTests;

[TestFixture]
public class WatchDogTests
{
    class Logger : ILogger<WatchDog>
    {
        public readonly List<string> Messages = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception));
    }

    private static Logger _logger = new Logger();

    private IWatchDogExecuter _watchdogExecuter = null!;
    private IWatchDog _watchdog = null!;

    private ServiceProvider _services = null!;

    private TaskCompletionSource _accessCounter = null!;

    private List<InterfaceLogPayload> _payloads = [];

    [SetUp]
    public void Setup()
    {
        _accessCounter = new();
        _payloads.Clear();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IWatchDogExecuter, WatchDogExecuter>();

        var testServer = new TestServer(new WebHostBuilder().Configure(app =>
        {
            app.Run(async context =>
            {
                if (context.Request.Path == "/watchdogTest.asp")
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<HTML><HEAD><TITLE>IP-WatchDog: ResetPage</TITLE></HEAD><BODY><B>ResetPage[3]</B><br>Page for method: OutgoingHTML page<br>Reset timer for IP addr: 192.168.32.16<br>Your IP address: 192.168.32.1<br></BODY></HTML>");
                }
                else if (context.Request.Path == "/not-a-watchdog.asp")
                {
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html>You shall not pass!</html>");
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            });
        }));

        if (TestContext.CurrentContext.Test.Name == "Fail_On_Connection_Refused")
        {
            var clientMock = new Mock<IHttpClientFactory>();
            clientMock.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());
            services.AddSingleton(clientMock.Object);
        }
        else
        {
            var clientMock = new Mock<IHttpClientFactory>();
            clientMock.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(testServer.CreateClient());
            services.AddSingleton(clientMock.Object);
        }

        services.AddSingleton<IWatchDogFactory, WatchDogFactory>();

        var prepareMock = new Mock<IPreparedInterfaceLogEntry>();
        prepareMock.Setup(p => p.Finish(It.IsAny<InterfaceLogPayload>())).Returns((InterfaceLogPayload p) => { _payloads.Add(p); if (_payloads.Count == 2) _accessCounter.SetResult(); return null; });

        var connectionMock = new Mock<IInterfaceConnection>();
        connectionMock.Setup(c => c.Prepare(It.IsAny<InterfaceLogEntryScope>())).Returns(prepareMock.Object);

        var loggerMock = new Mock<IInterfaceLogger>();
        loggerMock.Setup(l => l.CreateConnection(It.IsAny<InterfaceLogEntryConnection>())).Returns(connectionMock.Object);

        services.AddSingleton(loggerMock.Object);

        _services = services.BuildServiceProvider();

        _watchdogExecuter = _services.GetRequiredService<IWatchDogExecuter>();
    }

    [TearDown]
    public void Teardown()
    {
        _services.Dispose();
    }

    [Test, MaxTime(5000)]
    public async Task Can_Query_watchdog()
    {
        _services.GetRequiredService<IWatchDogFactory>().Initialize(new WatchDogConfiguration { EndPoint = "/watchdogTest.asp" });

        _watchdog = _services.GetRequiredService<IWatchDogFactory>().WatchDog!;

        await _accessCounter.Task;

        Assert.Multiple(() =>
       {
           Assert.That(_payloads.Count == 2);
           Assert.That(_payloads[1].Payload!.Contains("WatchDog Polled."));
       });
    }

    [Test, MaxTime(5000)]
    public async Task Fail_On_Connection_Refused()
    {
        _services.GetRequiredService<IWatchDogFactory>().Initialize(new WatchDogConfiguration { EndPoint = "http://localhost:0" });

        _watchdog = _services.GetRequiredService<IWatchDogFactory>().WatchDog!;

        await _accessCounter.Task;

        Assert.Multiple(() =>
       {
           Assert.That(_payloads.Count == 2);
           Assert.That(_payloads[1].TransferException!.Contains("Connection refused"));
       });
    }

    [Test, MaxTime(5000)]
    public async Task Fail_On_404()
    {
        _services.GetRequiredService<IWatchDogFactory>().Initialize(new WatchDogConfiguration { EndPoint = "/foo" });

        _watchdog = _services.GetRequiredService<IWatchDogFactory>().WatchDog!;

        await _accessCounter.Task;

        Assert.Multiple(() =>
       {
           Assert.That(_payloads.Count == 2);
           Assert.That(_payloads[1].TransferException!.Contains("404"));
       });
    }

    [Test, MaxTime(5000)]
    public async Task Fail_On_Wrong_Path()
    {
        _services.GetRequiredService<IWatchDogFactory>().Initialize(new WatchDogConfiguration { EndPoint = "/not-a-watchdog.asp" });

        _watchdog = _services.GetRequiredService<IWatchDogFactory>().WatchDog!;

        await _accessCounter.Task;

        Assert.Multiple(() =>
       {
           Assert.That(_payloads.Count == 2);
           Assert.That(_payloads[1].TransferException!.Contains("Site reachable, but is not WatchDog!"));
       });
    }
}

