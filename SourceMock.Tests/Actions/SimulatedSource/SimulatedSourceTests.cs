
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using SourceMock.Actions.Source;

namespace SourceMock.Tests.Actions.Source
{
    internal class SimulatedSourceTests
    {
        [Test]
        public void TestValidTurnOn()
        {
            // Arrange
            Mock<ILogger<SimulatedSource>> loggerMock = new();
            Mock<IConfiguration> configMock = new();

            ISource source = new SimulatedSource(loggerMock.Object, configMock.Object);

            source.SetLoadpoint(LoadpointValidator.LoadpointValidatorTestData.Loadpoint001_3AC_valid);

            // Act
            var result = source.TurnOn();

            // Assert
            Assert.AreEqual(SourceResult.SUCCESS, result);
        }

        [Test]
        public void TestValidTurnOff()
        {
            // Arrange
            Mock<ILogger<SimulatedSource>> loggerMock = new();
            Mock<IConfiguration> configMock = new();

            ISource source = new SimulatedSource(loggerMock.Object, configMock.Object);

            source.SetLoadpoint(LoadpointValidator.LoadpointValidatorTestData.Loadpoint001_3AC_valid);
            var result = source.TurnOn();

            // Act
            source.TurnOff();

            // Assert
            Assert.AreEqual(SourceResult.SUCCESS, result);

        }
    }
}