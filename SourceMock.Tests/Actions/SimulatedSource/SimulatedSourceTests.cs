
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using SourceMock.Actions.Source;
using SourceMock.Model;
using SourceMock.Tests.Actions.LoadpointValidator;

namespace SourceMock.Tests.Actions.Source
{
    internal class SimulatedSourceTests
    {
        const string CONFIG_KEY_NUMBER_OF_PHASES = "SourceProperties:NumberOfPhases";

        #region PositiveTestCases
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public void TestValidTurnOn(Loadpoint loadpoint)
        {
            // Arrange
            var configuration = BuildConfig(loadpoint.Currents.Count());
            Mock<ILogger<SimulatedSource>>? loggerMock = new();

            ISource source = new SimulatedSource(loggerMock.Object, configuration);

            source.SetLoadpoint(loadpoint);

            // Act
            var result = source.TurnOn();

            // Assert
            var nextLoadpoint = source.GetNextLoadpoint();
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.AreEqual(SourceResult.SUCCESS, result);
            Assert.AreEqual(loadpoint, nextLoadpoint);
            Assert.AreEqual(loadpoint, currentLoadpoint);
        }

        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public void TestValidTurnOff(Loadpoint loadpoint)
        {
            // Arrange
            var configuration = BuildConfig(loadpoint.Currents.Count());
            Mock<ILogger<SimulatedSource>>? loggerMock = new();

            ISource source = new SimulatedSource(loggerMock.Object, configuration);

            source.SetLoadpoint(loadpoint);
            var result = source.TurnOn();

            // Act
            source.TurnOff();

            // Assert
            var nextLoadpoint = source.GetNextLoadpoint();
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.AreEqual(SourceResult.SUCCESS, result);
            Assert.AreEqual(loadpoint, nextLoadpoint);
            Assert.AreEqual(null, currentLoadpoint);
        }
        #endregion

        #region LoadpointIssues
        [Test]
        public void TestTurnOnWithoutLoadpoint()
        {
            // Arrange
            ISource source = GenerateSimulatedSource();

            // Act
            var result = source.TurnOn();

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.AreEqual(SourceResult.NO_LOADPOINT_SET, result);
            Assert.AreEqual(null, currentLoadpoint);
        }

        [Test]
        [TestCaseSource(typeof(SimulatedSourceTestData), nameof(SimulatedSourceTestData.ValidLoadpointsWithOneOrThreePhases))]
        public void TestTurnOnWithInvalidLoadpoint(Loadpoint loadpoint)
        {
            const int numberOfPhases = 2;

            // Arrange
            var configuration = BuildConfig(numberOfPhases);
            Mock<ILogger<SimulatedSource>>? loggerMock = new();

            ISource source = new SimulatedSource(loggerMock.Object, configuration);

            // Act
            var result = source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.AreEqual(SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES, result);
            Assert.AreEqual(null, currentLoadpoint);
        }
        #endregion

        #region HelperFunctions
        private static ISource GenerateSimulatedSource(Mock<ILogger<SimulatedSource>>? loggerMock = null, Mock<IConfiguration>? configMock = null)
        {
            loggerMock ??= new();
            configMock ??= new();

            return new SimulatedSource(loggerMock.Object, configMock.Object);
        }

        private static IConfiguration BuildConfig(int numberOfPhases)
        {
            var inMemorySettings = new Dictionary<string, string> {
                {CONFIG_KEY_NUMBER_OF_PHASES, numberOfPhases.ToString()}
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
        #endregion
    }
}