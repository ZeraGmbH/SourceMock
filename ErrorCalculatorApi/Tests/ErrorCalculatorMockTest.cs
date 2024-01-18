using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SourceApi.Actions.Source;
using SourceApi.Model;

namespace ErrorCalculatorApiTests;

public class ErrorCalculatorMockTest
{
    [Test]
    public async Task Returns_Correct_Error_Status()
    {
        Mock<ISource> sourceMock = new();

        /* Total energy is 66kW. */
        Loadpoint loadpoint = new()
        {
            Phases = {
                new(){
                    Current = new(){Angle=0, Rms=100, On=true},
                    Voltage = new(){Angle=0, Rms=220, On=true}
                },
                new(){
                    Current = new(){Angle=120, Rms=100, On=true},
                    Voltage = new(){Angle=120, Rms=220, On=true}
                },
                new(){
                    Current = new(){Angle=240, Rms=100, On=true},
                    Voltage = new(){Angle=240, Rms=220, On=true}
                }
            }
        };

        sourceMock.Setup(s => s.GetCurrentLoadpoint()).Returns(loadpoint);

        var services = new ServiceCollection();
        services.AddSingleton(sourceMock.Object);
        using var provider = services.BuildServiceProvider();
        ErrorCalculatorMock mock = new(provider);

        /* 200 impulses at 10000/kWh is equivalent to 20. */
        await mock.SetErrorMeasurementParameters(10000, 200);
        await mock.StartErrorMeasurement(false);

        Thread.Sleep(100);

        var result = await mock.GetErrorStatus();

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Running));
            Assert.That(result.ErrorValue, Is.Null);
            Assert.That(result.Energy, Is.GreaterThan(0));
        });

        Thread.Sleep(1000);

        result = await mock.GetErrorStatus();

        Assert.Multiple(() =>
        {
            Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Finished));
            Assert.That(result.ErrorValue, Is.GreaterThanOrEqualTo(0.95).And.LessThanOrEqualTo(1.07));
            Assert.That(result.Energy, Is.EqualTo(20));
        });
    }
}
