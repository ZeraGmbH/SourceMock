using MeterTestSystemApi.Actions.Probing;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework.Internal;
using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class ProbeSerialTests
{
    private class PortMock : ISerialPort
    {
        private readonly Queue<string> _replies = [];

        public void Dispose()
        {
        }

        public byte? RawRead()
        {
            throw new NotImplementedException();
        }

        public void RawWrite(byte[] command)
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            if (_replies.TryDequeue(out var reply))
                return reply;

            Thread.Sleep(100);

            throw new TimeoutException("queue is empty");
        }

        public void WriteLine(string command)
        {
            switch (command)
            {
                case "TS":
                    _replies.Enqueue("TSFG312   V265");
                    break;
                case "AAV":
                    _replies.Enqueue("MT712V04.99");
                    _replies.Enqueue("AAVACK");
                    break;
                case "AV":
                    _replies.Enqueue("EBV99.13");
                    _replies.Enqueue("JMS");
                    _replies.Enqueue("AVACK");
                    break;
            }
        }
    }

    private ServiceProvider Services = null!;

    private IProbeConfigurationService Prober = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddSingleton<IInterfaceLogger, NoopInterfaceLogger>();

        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();

        services.AddKeyedTransient<IProbeExecutor, ProbeSerialPort>(typeof(SerialProbe));
        services.AddKeyedTransient<ISerialPortProbeExecutor, ESxBSerialPortProbing>(SerialProbeProtocols.ESxB);
        services.AddKeyedTransient<ISerialPortProbeExecutor, FGSerialPortProbing>(SerialProbeProtocols.FG30x);
        services.AddKeyedTransient<ISerialPortProbeExecutor, MTSerialPortProbing>(SerialProbeProtocols.MT768);
        services.AddKeyedTransient<ISerialPortProbeExecutor, ZIFSerialPortProbing>(SerialProbeProtocols.PM8181);

        var connectionMock = new Mock<ISerialPortConnectionForProbing>();

        connectionMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<SerialPortOptions>(), It.IsAny<bool>())).Returns(
            (string port, SerialPortOptions? options, bool enableReader) =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options!.ReadTimeout, Is.EqualTo(2000));
                Assert.That(options.WriteTimeout, Is.EqualTo(2000));

                return SerialPortConnection.FromMockedPortInstance(new PortMock(), new NullLogger<SerialPortConnection>(), enableReader, options.ReadTimeout);
            }
        );

        services.AddSingleton(connectionMock.Object);

        Services = services.BuildServiceProvider();

        Prober = Services.GetRequiredService<IProbeConfigurationService>();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [TestCase(SerialProbeProtocols.MT768, 0, SerialPortTypes.USB, true, "MT Model MT712")]
    [TestCase(SerialProbeProtocols.PM8181, 1, SerialPortTypes.RS232, false, "/dev/ttyS1: PM8181: The method or operation is not implemented.")]
    [TestCase(SerialProbeProtocols.FG30x, 2, SerialPortTypes.USB, true, "FG Model FG312")]
    [TestCase(SerialProbeProtocols.ESxB, 3, SerialPortTypes.RS232, true, "EBV99.13")]
    public async Task Can_Probe_Serial_Port(SerialProbeProtocols protocol, int index, SerialPortTypes type, bool implemented, string message)
    {
        var results = await Prober.ProbeManualAsync([new SerialProbe {
            Device = new() { Index = checked((uint)index), Type = type},
            Protocol = protocol,
            Result = ProbeResult.Planned
        }], Services);

        Assert.Multiple(() =>
        {
            Assert.That(results[0].Succeeded, Is.EqualTo(implemented));
            Assert.That(results[0].Message, Is.EqualTo(message));
        });
    }
}