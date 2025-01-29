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
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class ProbeSerialTests
{
    private class PortMock : ISerialPort
    {
        private readonly Queue<string> _replies = [];

        private readonly Queue<byte> _rawReplies = [];

        public void Dispose()
        {
        }

        public byte? RawRead(int? timeout = null)
        {
            if (_rawReplies.TryDequeue(out var reply))
                return reply;

            return null;
        }

        public void RawWrite(byte[] command)
        {
            switch (BitConverter.ToString(command))
            {
                case "A5-02-C2-E7-5A":
                    Array.ForEach<byte>([0xa5, 0x08, 0x06, 0xc2, 0x02, 0x00, 0x00, 0x00, 0x16, 0x96, 0x5a], _rawReplies.Enqueue);
                    break;
            }
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

    private int? BaudRateOption = null;

    [SetUp]
    public void Setup()
    {
        BaudRateOption = null;

        var services = new ServiceCollection();

        services.AddLogging();

        services.AddSingleton<IInterfaceLogger, NoopInterfaceLogger>();

        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();

        services.AddKeyedScoped<IProbeExecutor, ProbeSerialPort>(typeof(SerialProbe));
        services.AddKeyedScoped<IProbeExecutor, ProbeSerialPortOverTcp>(typeof(SerialProbeOverTcp));
        services.AddKeyedTransient<ISerialPortProbeExecutor, ESxBSerialPortProbing>(SerialProbeProtocols.ESxB);
        services.AddKeyedTransient<ISerialPortProbeExecutor, FGSerialPortProbing>(SerialProbeProtocols.FG30x);
        services.AddKeyedTransient<ISerialPortProbeExecutor, MTSerialPortProbing>(SerialProbeProtocols.MT768);
        services.AddKeyedTransient<ISerialPortProbeExecutor, ZIFSerialPortProbing>(SerialProbeProtocols.PM8181);

        var connectionMock = new Mock<ISerialPortConnectionForProbing>();

        connectionMock.Setup(f => f.CreatePhysical(It.IsAny<string>(), It.IsAny<SerialPortOptions>(), It.IsAny<bool>())).Returns(
            (string port, SerialPortOptions? options, bool enableReader) =>
            {
                Assert.That(options, Is.Not.Null);
                Assert.That(options!.ReadTimeout, Is.EqualTo(2000));
                Assert.That(options.WriteTimeout, Is.EqualTo(2000));
                Assert.That(options.BaudRate, Is.EqualTo(BaudRateOption));

                return SerialPortConnection.FromMockedPortInstance(new PortMock(), new NullLogger<SerialPortConnection>(), enableReader, options.ReadTimeout);
            }
        );

        connectionMock.Setup(f => f.CreateNetwork(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>())).Returns(
           (string endpoint, int? readTimeout, bool enableReader) =>
           {
               Assert.That(readTimeout, Is.EqualTo(2000));
               Assert.That(endpoint, Is.EqualTo("some-server:12983"));

               return SerialPortConnection.FromMockedPortInstance(new PortMock(), new NullLogger<SerialPortConnection>(), enableReader);
           }
       );

        services.AddSingleton(connectionMock.Object);

        services.AddSingleton(new Mock<IServerLifetime>().Object);
        services.AddSingleton(new Mock<IActiveOperations>().Object);

        Services = services.BuildServiceProvider();

        Prober = Services.GetRequiredService<IProbeConfigurationService>();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [TestCase(SerialProbeProtocols.MT768, 0, SerialPortTypes.USB, "MT Model MT712")]
    [TestCase(SerialProbeProtocols.PM8181, 1, SerialPortTypes.RS232, "PowerMaster8121 ZIF Version 2.22")]
    [TestCase(SerialProbeProtocols.FG30x, 2, SerialPortTypes.USB, "FG Model FG312")]
    [TestCase(SerialProbeProtocols.ESxB, 3, SerialPortTypes.RS232, "ESxB Version EBV99.13")]
    public async Task Can_Probe_Serial_Port_Async(SerialProbeProtocols protocol, int index, SerialPortTypes type, string message)
    {
        var results = await Prober.ProbeManualAsync([new SerialProbe {
            Device = new() { Index = checked((uint)index), Type = type},
            Protocol = protocol,
            Result = ProbeResult.Planned
        }], Services);

        Assert.Multiple(() =>
        {
            Assert.That(results[0].Succeeded, Is.True);
            Assert.That(results[0].Message, Is.EqualTo(message));
        });
    }

    [TestCase(SerialProbeProtocols.MT768, "MT Model MT712")]
    [TestCase(SerialProbeProtocols.PM8181, "PowerMaster8121 ZIF Version 2.22")]
    [TestCase(SerialProbeProtocols.FG30x, "FG Model FG312")]
    [TestCase(SerialProbeProtocols.ESxB, "ESxB Version EBV99.13")]
    public async Task Can_Probe_Serial_Port_Over_Network_Async(SerialProbeProtocols protocol, string message)
    {
        var results = await Prober.ProbeManualAsync([new SerialProbeOverTcp {
                Endpoint = "some-server:12983",
                Protocol = protocol,
                Result = ProbeResult.Planned
            }], Services);

        Assert.Multiple(() =>
        {
            Assert.That(results[0].Succeeded, Is.True);
            Assert.That(results[0].Message, Is.EqualTo(message));
        });
    }

    [Test]
    public async Task Can_Provide_Options_Async()
    {
        BaudRateOption = 14440;

        var results = await Prober.ProbeManualAsync([new SerialProbe {
            Device = new() { Index = 0, Type = SerialPortTypes.USB, Options = new() { BaudRate = 14440 } },
            Protocol = SerialProbeProtocols.FG30x,
            Result = ProbeResult.Planned
        }], Services);

        Assert.Multiple(() =>
       {
           Assert.That(results[0].Succeeded, Is.True);
           Assert.That(results[0].Message, Is.EqualTo("FG Model FG312"));
       });
    }

    [Test]
    public async Task Can_Skip_Probe_Async()
    {
        var results = await Prober.ProbeManualAsync([new SerialProbe {
            Device = new() { Index = 0, Type = SerialPortTypes.USB },
            Protocol = SerialProbeProtocols.FG30x,
            Result = ProbeResult.Skipped
        }], Services);

        Assert.That(results[0].Message, Is.EqualTo("Excluded from probing"));
    }
}