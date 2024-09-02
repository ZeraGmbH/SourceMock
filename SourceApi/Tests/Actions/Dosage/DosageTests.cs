
using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;
using ZERA.WebSam.Shared.Actions;
using SourceApi.Actions.SerialPort;
using SourceApi.Actions.SerialPort.MT768;
using SourceApi.Actions.Source;
using SourceApi.Tests.Actions.Dosage.PortMocks;

namespace SourceApi.Tests.Actions.Dosage;

[TestFixture]
public class DosageTests
{
    static DosageTests()
    {
        SerialPortConnection.ActivateUnitTestMode(30000);
    }

    private readonly NullLogger<ISerialPortConnection> _portLogger = new();

    private readonly DeviceLogger<SerialPortMTSource> _deviceLogger = new();

    private ISource CreateDevice(params string[] replies) => new SerialPortMTSource(_deviceLogger, SerialPortConnection.FromMockedPortInstance(new FixedReplyMock(replies), _portLogger), new CapabilitiesMap(), new SourceCapabilityValidator());

    [Test]
    public async Task Can_Turn_Off_DOS_Mode_Async()
    {
        await CreateDevice(new[] { "SOK3CM4" }).SetDosageMode(new NoopInterfaceLogger(), false);
    }

    [Test]
    public async Task Can_Turn_On_DOS_Mode_Async()
    {
        await CreateDevice(new[] { "SOK3CM3" }).SetDosageMode(new NoopInterfaceLogger(), true);
    }

    [Test]
    public async Task Can_Start_Dosage_Async()
    {
        await CreateDevice(new[] { "SOK3CM1" }).StartDosage(new NoopInterfaceLogger());
    }

    [Test]
    public async Task Can_Abort_Dosage_Async()
    {
        await CreateDevice(new[] { "SOK3CM2" }).CancelDosage(new NoopInterfaceLogger());
    }

    [TestCase(2, "113834")]
    [TestCase(1, "113834")]
    [TestCase(0, "113834")]
    [TestCase(2, "0")]
    [TestCase(2, "330E-1")]
    [TestCase(2, "333E2")]
    public async Task Can_Read_Dosage_Progress_Async(int dosage, string remaining)
    {
        /* Warning: knows about internal sequence of requests. */
        var progress = await CreateDevice(new[] {
            $"SOK3SA1;{dosage}",
            $"SOK3MA4;{remaining}",
            "SOK3MA5;303541",
            "SOK3SA5;218375",
        }).GetDosageProgress(new NoopInterfaceLogger(), new(600000000d));

        Assert.Multiple(() =>
        {
            Assert.That(progress.Active, Is.EqualTo(dosage == 2));
            Assert.That((double)progress.Remaining, Is.EqualTo(double.Parse(remaining) / 600000d));
            Assert.That((double)progress.Progress, Is.EqualTo(303541 / 600000d));
            Assert.That((double)progress.Total, Is.EqualTo(218375 / 600000d));
        });
    }

    [TestCase(1)]
    [TestCase(1.23)]
    [TestCase(1E5)]
    [TestCase(1E-5)]
    [TestCase(3)]
    public async Task Can_Set_Impules_From_Energy_Async(double energy)
    {
        var mock = new CommandPeekMock(new[] {
            "SOK3PS46"
        });

        var device = new SerialPortMTSource(_deviceLogger, SerialPortConnection.FromMockedPortInstance(mock, _portLogger), new CapabilitiesMap(), new SourceCapabilityValidator());

        await device.SetDosageEnergy(new NoopInterfaceLogger(), new(energy), new(600000000d));

        Assert.That(mock.Commands[0], Is.EqualTo($"S3PS46;{(long)(6000E5 * energy / 1000d):0000000000}"));
    }
}
