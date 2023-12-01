
using Microsoft.Extensions.Logging.Abstractions;

using SerialPortProxy;

using WebSamDeviceApis.Actions.SerialPort.MT768;
using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Tests.Actions.Dosage.PortMocks;

namespace WebSamDeviceApis.Tests.Actions.Dosage;

[TestFixture]
public class DosageTests
{
    private readonly NullLogger<SerialPortConnection> _portLogger = new();

    private readonly DeviceLogger _deviceLogger = new();

    private ISource CreateDevice(params string[] replies) => new SerialPortMTSource(_deviceLogger, SerialPortConnection.FromPortInstance(new FixedReplyMock(replies), _portLogger));

    [Test]
    public async Task Can_Turn_Off_DOS_Mode()
    {
        await CreateDevice(new[] { "SOK3CM4" }).SetDosageMode(false);
    }

    [Test]
    public async Task Can_Turn_On_DOS_Mode()
    {
        await CreateDevice(new[] { "SOK3CM3" }).SetDosageMode(true);
    }

    [Test]
    public async Task Can_Start_Dosage()
    {
        await CreateDevice(new[] { "SOK3CM1" }).StartDosage();
    }

    [Test]
    public async Task Can_Abort_Dosage()
    {
        await CreateDevice(new[] { "SOK3CM2" }).CancelDosage();
    }

    [TestCase(2, "113834")]
    [TestCase(1, "113834")]
    [TestCase(0, "113834")]
    [TestCase(2, "0")]
    [TestCase(2, "330E-1")]
    [TestCase(2, "333E2")]
    public async Task Can_Read_Dosage_Progress(int dosage, string remaining)
    {
        /* Warning: knows about internal sequence of requests. */
        var progress = await CreateDevice(new[] {
            $"SOK3SA1;{dosage}",
            $"SOK3MA4;{remaining}",
            "SOK3MA5;303541",
            "SOK3SA5;218375",
            "UB=60",
            "IB=2",
            "M=4WA",
            "ASTACK",
        }).GetDosageProgress();

        Assert.Multiple(() =>
        {
            Assert.That(progress.Active, Is.EqualTo(dosage == 2));
            Assert.That(progress.Remaining, Is.EqualTo(double.Parse(remaining) / 600000d));
            Assert.That(progress.Progress, Is.EqualTo(303541 / 600000d));
            Assert.That(progress.Total, Is.EqualTo(218375 / 600000d));
        });
    }

    [TestCase(1)]
    [TestCase(1.23)]
    [TestCase(1E5)]
    [TestCase(1E-5)]
    [TestCase(3)]
    public async Task Can_Set_Impules_From_Energy(double energy)
    {
        var mock = new CommandPeekMock(new[] {
            "UB=60",
            "IB=2",
            "M=4WA",
            "ASTACK",
            "SOK3PS46"
        });

        var device = new SerialPortMTSource(_deviceLogger, SerialPortConnection.FromPortInstance(mock, _portLogger));

        await device.SetDosageEnergy(energy);

        Assert.That(mock.Commands[1], Is.EqualTo($"S3PS46;{(long)(6000E5 * energy / 1000d):0000000000}"));
    }
}
