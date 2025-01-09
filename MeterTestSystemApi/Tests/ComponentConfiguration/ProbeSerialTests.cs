using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class ProbeSerialTests
{
    private ServiceProvider Services = null!;

    private IProbeConfigurationService Prober = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();

        services.AddKeyedTransient<IProbeExecutor, ProbeSerialPort>(typeof(SerialProbe));
        services.AddKeyedTransient<ISerialPortProbeExecutor, ESxBSerialPortProbing>(SerialProbeProtocols.ESxB);
        services.AddKeyedTransient<ISerialPortProbeExecutor, FGSerialPortProbing>(SerialProbeProtocols.FG30x);
        services.AddKeyedTransient<ISerialPortProbeExecutor, MTSerialPortProbing>(SerialProbeProtocols.MT768);
        services.AddKeyedTransient<ISerialPortProbeExecutor, ZIFSerialPortProbing>(SerialProbeProtocols.PM8181);

        Services = services.BuildServiceProvider();

        Prober = Services.GetRequiredService<IProbeConfigurationService>();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Ignore("work in progress")]
    [TestCase(SerialProbeProtocols.MT768, 0, SerialPortTypes.USB)]
    [TestCase(SerialProbeProtocols.PM8181, 1, SerialPortTypes.RS232)]
    [TestCase(SerialProbeProtocols.FG30x, 2, SerialPortTypes.USB)]
    [TestCase(SerialProbeProtocols.ESxB, 3, SerialPortTypes.RS232)]
    public async Task Can_Probe_Serial_Port(SerialProbeProtocols protocol, int index, SerialPortTypes type)
    {
        var results = await Prober.ProbeManualAsync([new SerialProbe {
            Device = new() { Index = checked((uint)index), Type = type},
            Protocol = protocol,
            Result = ProbeResult.Planned
        }]);

        Assert.That(results[0], Is.EqualTo(""));
    }
}