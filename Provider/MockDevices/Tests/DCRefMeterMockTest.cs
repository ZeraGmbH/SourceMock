using Microsoft.Extensions.DependencyInjection;
using MockDevices.ReferenceMeter;
using Moq;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;
using ZERA.WebSam.Shared.Models.ReferenceMeter;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Provider;

namespace MockDeviceTests;

public class DCRefMeterMockTest
{

    private ServiceProvider Services;

    private Mock<ISource> SourceMock;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        SourceMock = new Mock<ISource>();

        SourceMock.Setup(s => s.GetAvailableAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(true);

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

        MeasuredLoadpoint measureOutput = await refMeterMock.GetActualValuesAsync(new NoopInterfaceLogger());

        Assert.That((double?)measureOutput.Phases[0].Current.DcComponent, Is.InRange(0, 0.01));
    }


    [Test]
    public async Task Produces_Actual_Values_If_Loadpoint_Switched_On_Async()
    {
        double current = 22222222;
        double voltage = 33333333;

        SourceMock.Setup(s => s.GetAvailableAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(true);
        SourceMock.Setup(s => s.GetCurrentLoadpointAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(
            new TargetLoadpoint()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new(){DcComponent = new(current), On=true},
                        Voltage = new(){DcComponent = new(voltage), On=true}
                    }
                }
            });

        SourceMock.Setup(s => s.GetActiveLoadpointInfoAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(new LoadpointInfo { IsActive = true });
        SourceMock.Setup(s => s.CurrentSwitchedOffForDosageAsync(It.IsAny<IInterfaceLogger>())).ReturnsAsync(false);

        DCRefMeterMock refMeterMock = new(Services);
        MeasuredLoadpoint measureOutput = await refMeterMock.GetActualValuesAsync(new NoopInterfaceLogger());

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
