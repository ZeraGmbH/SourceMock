using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using RefMeterApi.Actions.Device;
using RefMeterApi.Controllers;
using RefMeterApi.Models;
using RefMeterApiTests.PortMocks;
using SerialPortProxy;

namespace RefMeterApiTests;

[TestFixture]
public class RefMeterControllerTests
{
    private readonly NullLogger<ISerialPortConnection> _portLogger = new();

    private readonly NullLogger<SerialPortMTRefMeter> _deviceLogger = new();

    [Test]
    public async Task Controller_Will_Decode_AME_Reply()
    {
        using var port = SerialPortConnection.FromMock<StandardPortMock>(_portLogger);

        var cut = new RefMeterController(new SerialPortMTRefMeter(port, _deviceLogger));

        var response = await cut.GetActualValues();
        var result = response.Result as OkObjectResult;
        var data = result?.Value as MeasuredLoadpoint;

        Assert.That(data, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(data.Frequency, Is.EqualTo(50).Within(0.5));
            Assert.That(data.Phases[0].Voltage.AcComponent!.Rms, Is.EqualTo(20).Within(0.5));
            Assert.That(data.Phases[1].Current.AcComponent!.Rms, Is.EqualTo(0.1).Within(0.05));
            Assert.That(data.Phases[1].Voltage.AcComponent!.Angle, Is.EqualTo(240).Within(0.5));
            Assert.That(data.Phases[2].Current.AcComponent!.Angle, Is.EqualTo(120).Within(0.5));

            Assert.That(data.PhaseOrder, Is.EqualTo("123"));
        });
    }
}
