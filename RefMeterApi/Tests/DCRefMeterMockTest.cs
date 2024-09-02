using Microsoft.Extensions.DependencyInjection;
using Moq;
using RefMeterApi.Actions.Device;
using RefMeterApi.Models;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace RefMeterApiTests;

public class DCRefMeterMockTest
{

    private ServiceProvider Services;

    private Mock<ISource> SourceMock;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        SourceMock = new Mock<ISource>();

        SourceMock.Setup(s => s.GetAvailable(It.IsAny<IInterfaceLogger>())).Returns(true);

        services.AddSingleton(SourceMock.Object);

        Services = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }


    [Test]
    public async Task Produces_Actual_Values_If_No_Loadpoint_Switched_On_Async()
    {
        DCRefMeterMock refMeterMock = new(Services);

        MeasuredLoadpoint measureOutput = await refMeterMock.GetActualValues(new NoopInterfaceLogger());

        Assert.That((double?)measureOutput.Phases[0].Current.DcComponent, Is.InRange(0, 0.01));
    }


    [Test]
    public async Task Produces_Actual_Values_If_Loadpoint_Switched_On_Async()
    {
        double current = 22222222;
        double voltage = 33333333;

        SourceMock.Setup(s => s.GetAvailable(It.IsAny<IInterfaceLogger>())).Returns(true);
        SourceMock.Setup(s => s.GetCurrentLoadpoint(It.IsAny<IInterfaceLogger>())).Returns(
            new TargetLoadpoint()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new(){DcComponent = new(current), On=true},
                        Voltage = new(){DcComponent = new(voltage), On=true}
                    }
                }
            });

        SourceMock.Setup(s => s.GetActiveLoadpointInfo(It.IsAny<IInterfaceLogger>())).Returns(new LoadpointInfo { IsActive = true });
        SourceMock.Setup(s => s.CurrentSwitchedOffForDosage(It.IsAny<IInterfaceLogger>())).ReturnsAsync(false);

        DCRefMeterMock refMeterMock = new(Services);
        MeasuredLoadpoint measureOutput = await refMeterMock.GetActualValues(new NoopInterfaceLogger());

        Assert.That((double?)measureOutput.Phases[0].Current.DcComponent, Is.InRange(GetMinValue(current, 0.01), GetMaxValue(current, 0.01)));
        Assert.That((double?)measureOutput.Phases[0].Voltage.DcComponent, Is.InRange(GetMinValue(voltage, 0.01), GetMaxValue(voltage, 0.01)));
    }

    [Test]
    public void Transfers_Data_Correctly_To_Measure_Output()
    {
        double current = 234;
        double voltage = 456;
        double apparentPower = current * voltage;

        // Arrange
        TargetLoadpoint lp = new TargetLoadpoint()
        {
            Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new(){DcComponent = new(current), On=true},
                        Voltage = new(){DcComponent = new(voltage), On=true}
                    }
                }
        };

        DCRefMeterMock refMeterMock = new(Services);

        // Act
        var mo = refMeterMock.CalcMeasureOutput(lp);

        // Assert
        Assert.That((double?)mo.Phases[0].Voltage.DcComponent, Is.InRange(GetMinValue(voltage, 0.01), GetMaxValue(voltage, 0.01)));
        Assert.That((double?)mo.Phases[0].Current.DcComponent, Is.InRange(GetMinValue(current, 0.01), GetMaxValue(current, 0.01)));
        Assert.That((double?)mo.Phases[0].ActivePower, Is.InRange(GetMinValue(current, 0.01), GetMaxValue(apparentPower, 0.02)));
    }


    private double GetMinValue(double value, double deviation)
    {
        return value - value * deviation;
    }

    private double GetMaxValue(double value, double deviation)
    {
        return value + value * deviation;
    }
}
