using System.Reflection;
using BarcodeApi.Actions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BarcodeApiTests;

[TestFixture]
public class InputDeviceTests
{
    private static readonly string executableDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

    private ServiceProvider Services = null!;

    private IInputDeviceManager Devices = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        var deviceMock = new Mock<InputDeviceManager.IInputDeviceProvider>();
        var mockedDevices = File.ReadAllText(Path.Combine(executableDir, "TestData", "input-devices"));

        deviceMock.SetupGet(s => s.DeviceFile).Returns(mockedDevices);

        services.AddSingleton(deviceMock.Object);

        services.AddTransient<IInputDeviceManager, InputDeviceManager>();

        Services = services.BuildServiceProvider();

        Devices = Services.GetRequiredService<IInputDeviceManager>();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Can_Read_All_Devices_Async()
    {
        var list = await Devices.GetAsync();

        Assert.That(list, Has.Count.EqualTo(27));
    }

    [Test]
    public async Task Can_Read_All_Keyboard_HID_Devices_Async()
    {
        var list = await Devices.GetKeyboardHIDDevices();

        Assert.That(list, Has.Count.EqualTo(15));
    }

    [Test]
    public async Task Can_Read_All_Barcode_Candidate_Devices_Async()
    {
        var list = await Devices.GetHIDBarcodeCandidateDevices(20);

        Assert.That(list, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(list[0].GetProperty("Name"), Is.EqualTo("SM SM-2D PRODUCT HID KBW"));
            Assert.That(list[1].GetProperty("Name"), Is.EqualTo("WSM Keyboard"));
        });
    }
}