using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WatchDogApi.Models;
using WatchDogApi.Services;
using Moq;

namespace WatchDogApiTests;

[TestFixture]
public class WatchDogExecuterTests
{
    private IWatchDogExecuter _watchdog = null!;

    private ServiceProvider _services = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IWatchDogExecuter, WatchDogExecuter>();

        // for specific tests inject a "normal" http client (connection refused tests etc.)
        if (TestContext.CurrentContext.Test.Name == "Query_Non_Existant_Server")
        {
            var clientMock = new Mock<IHttpClientFactory>();
            clientMock.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient());
            services.AddSingleton(clientMock.Object);
        }
        else
        {
            var clientMock = new Mock<IHttpClientFactory>();
            clientMock.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(() => new TestServer(new WebHostBuilder().Configure(app =>
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
                });
            })).CreateClient());
            services.AddSingleton(clientMock.Object);
        }

        _services = services.BuildServiceProvider();

        _watchdog = _services.GetRequiredService<IWatchDogExecuter>();
    }

    [TearDown]
    public void Teardown()
    {
        _services.Dispose();
    }

    [Test]
    public async Task Fails_On_Querying_Wrong_Address()
    {
        var result = await _watchdog.QueryWatchDogSingleEndpointAsync("/foo");

        Assert.That(!result);
    }
    [Test]
    public async Task Fails_On_Querying_Wrong_Response()
    {
        var result = await _watchdog.QueryWatchDogSingleEndpointAsync("/not-a-watchdog.asp");

        Assert.That(!result);
    }

    [Test]
    public async Task Can_Successfully_Query_Server()
    {
        var result = await _watchdog.QueryWatchDogSingleEndpointAsync("/watchdogTest.asp");

        Assert.That(result);
    }

    [Test]
    public void Query_Non_Existant_Server()
    {
        Assert.ThrowsAsync<HttpRequestException>(async () => await _watchdog.QueryWatchDogSingleEndpointAsync("http://localhost:0"));
    }

    [Test]
    public void Can_Build_Endpoint_List()
    {
        var result = _watchdog.BuildHttpEndpointList("127.0.0.1", 2);
        Assert.Multiple(() =>
        {
            Assert.That(result[0], Is.EqualTo("http://127.0.0.1/cgi-bin/refreshpage1.asp"));
            Assert.That(result[1], Is.EqualTo("http://127.0.0.1/cgi-bin/refreshpage2.asp"));
        });
    }
}

