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

        Loadpoint loadpoint = new()
        {
            Phases = new List<PhaseLoadpoint>(){
                new PhaseLoadpoint(){
                    Current = new(){Angle=0, Rms=10},
                    Voltage = new(){Angle=0, Rms=220}
                },
                new PhaseLoadpoint(){
                    Current = new(){Angle=120, Rms=10},
                    Voltage = new(){Angle=120, Rms=220}
                },
                new PhaseLoadpoint(){
                    Current = new(){Angle=240, Rms=10},
                    Voltage = new(){Angle=240, Rms=220}
                }
            }
        };

        sourceMock.Setup(s => s.GetCurrentLoadpoint()).Returns(loadpoint);

        var services = new ServiceCollection();
        services.AddSingleton(sourceMock.Object);
        using var provider = services.BuildServiceProvider();
        ErrorCalculatorMock mock = new(provider);

        await mock.SetErrorMeasurementParameters(10000, 200);
        await mock.StartErrorMeasurement(false);

        ErrorMeasurementStatus result = await mock.GetErrorStatus();

        Assert.That(result.State, Is.EqualTo(ErrorMeasurementStates.Running));
        Assert.That(result.ErrorValue, Is.Null);
        Assert.That(result.Energy, Is.GreaterThan(0));
    }
}
