using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedLibrary.Actions;
using SharedLibrary.Models.Logging;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace ErrorCalculatorApiTests;

public class ErrorCalculatorMockTest
{
    private ServiceProvider Services;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        Mock<ISource> sourceMock = new();

        /* Total energy is 66kW. */
        TargetLoadpoint loadpoint = new()
        {
            Phases = {
                new(){
                    Current = new(){AcComponent = new() {Angle=0, Rms=100}, On=true},
                    Voltage = new(){AcComponent = new() {Angle=0, Rms=220}, On=true}
                },
                new(){
                    Current = new(){AcComponent = new() {Angle=120, Rms=100}, On=true},
                    Voltage = new(){AcComponent = new() {Angle=120, Rms=220}, On=true}
                },
                new(){
                    Current = new(){AcComponent = new() {Angle=240, Rms=100}, On=true},
                    Voltage = new(){AcComponent = new() {Angle=240, Rms=220}, On=true}
                }
            }
        };

        sourceMock.Setup(s => s.GetCurrentLoadpoint(It.IsAny<IInterfaceLogger>())).Returns(loadpoint);

        services.AddSingleton(sourceMock.Object);

        services.AddTransient<IErrorCalculatorMock, ErrorCalculatorMock>();

        Services = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Returns_Correct_Error_Status()
    {
        var cut = Services.GetRequiredService<IErrorCalculatorMock>();

        /* 200 impulses at 10000/kWh is equivalent to 20. */
        await cut.SetErrorMeasurementParameters(new NoopInterfaceLogger(), 10000, 200, 6000d);
        await cut.StartErrorMeasurement(new NoopInterfaceLogger(), false, null);

        /* 100ms delay generates ~1.8W. */
        Thread.Sleep(100);

        var result = await cut.GetErrorStatus(new NoopInterfaceLogger());

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Running));
            Assert.That(result.ErrorValue, Is.Null);
            Assert.That(result.ReferenceCounts, Is.Null);
        });

        /* 1.5s delay generates 27.5W. */
        Thread.Sleep(1500);

        result = await cut.GetErrorStatus(new NoopInterfaceLogger());

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Finished));
            Assert.That(result.ErrorValue, Is.GreaterThanOrEqualTo(-5).And.LessThanOrEqualTo(+7));
            Assert.That(result.ReferenceCounts, Is.Null);
        });
    }

    [Test]
    public async Task Can_Do_Continuous_Measurement()
    {
        var cut = Services.GetRequiredService<IErrorCalculatorMock>();

        /* 200 impulses at 10000/kWh is equivalent to 20W. */
        await cut.SetErrorMeasurementParameters(new NoopInterfaceLogger(), 10000, 200, 6000d);
        await cut.StartErrorMeasurement(new NoopInterfaceLogger(), true, null);

        /* 1.5s delay generates 27.5W. */
        Thread.Sleep(1500);

        var result = await cut.GetErrorStatus(new NoopInterfaceLogger());
        var error = result.ErrorValue;

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Running));
            Assert.That(error, Is.GreaterThanOrEqualTo(-5).And.LessThanOrEqualTo(+7));
            Assert.That(result.ReferenceCounts, Is.Null);
        });

        Thread.Sleep(100);

        result = await cut.GetErrorStatus(new NoopInterfaceLogger());

        /* There should be no full cycle be done and the error value keeps its value. */
        Assert.That(error, Is.EqualTo(result.ErrorValue));

        /* 1.5s delay generates 27.5W. */
        Thread.Sleep(1500);

        result = await cut.GetErrorStatus(new NoopInterfaceLogger());

        /* Now the second iteration should be complete and the error value "should" change - indeed there is a tin chance of random generation clashes. */
        Assert.That(error, Is.Not.EqualTo(result.ErrorValue));
    }
}
