
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using SourceMock.Actions.Source;

namespace SourceMock.Tests.Actions.Source
{
    internal class SimulatedSourceTests
    {
        #region PositiveTestCases
        [Test]
        public void TestValidTurnOn()
        {
            // Arrange
            ISource source = GenerateSimulatedSourceWithStandardMocks();
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
            ISource source = GenerateSimulatedSourceWithStandardMocks();

            source.SetLoadpoint(LoadpointValidator.LoadpointValidatorTestData.Loadpoint001_3AC_valid);
            var result = source.TurnOn();

            // Act
            source.TurnOff();

            // Assert
            Assert.AreEqual(SourceResult.SUCCESS, result);
        }
        #endregion

        #region LoadpointIssues
        [Test]
        public void TestTurnOnWithoutLoadpoint()
        {
            // Arrange
            ISource source = GenerateSimulatedSourceWithStandardMocks();

            // Act
            var result = source.TurnOn();

            // Assert
            Assert.AreEqual(SourceResult.NO_LOADPOINT_SET, result);
        }

        public void TestTurnOnWithInvalidLoadpoint()
        {

        }
        #endregion

        private ISource GenerateSimulatedSourceWithStandardMocks()
        {
            Mock<ILogger<SimulatedSource>> loggerMock = new();
            Mock<IConfiguration> configMock = new();

            return new SimulatedSource(loggerMock.Object, configMock.Object);
        }
    }
}