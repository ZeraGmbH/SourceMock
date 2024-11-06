using BurdenApi.Actions;
using BurdenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApiTests;

[TestFixture]
public class BurdenTests
{
    private readonly IInterfaceLogger Logger = new NoopInterfaceLogger();

    private ServiceProvider Services = null!;

    private IBurden Burden = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        var device = SerialPortConnection.FromMock<BurdenSerialPortMock>(new NullLogger<SerialPortConnection>());

        services.AddKeyedSingleton("Burden", device);

        services.AddTransient<IBurden, Burden>();

        Services = services.BuildServiceProvider();

        Burden = Services.GetRequiredService<IBurden>();
    }

    [TearDown]
    public void TearDown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Can_Retrieve_Version_Async()
    {
        var version = await Burden.GetVersionAsync(Logger);

        Assert.Multiple(() =>
        {
            Assert.That(version.Version, Is.EqualTo("EBV33.12"));
            Assert.That(version.Supplement, Is.EqualTo("XB"));
        });
    }
}