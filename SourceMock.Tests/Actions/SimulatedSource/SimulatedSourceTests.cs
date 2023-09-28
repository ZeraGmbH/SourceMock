
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
            var capabilities = SimulatedSourceTestData.DefaultThreePhaseSourceCapabilities;
            capabilities.NumberOfPhases = loadpoint.Currents.Count();

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

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
            var capabilities = SimulatedSourceTestData.DefaultThreePhaseSourceCapabilities;
            capabilities.NumberOfPhases = loadpoint.Currents.Count();

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

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
            // Arrange
            var capabilities = SimulatedSourceTestData.DefaultThreePhaseSourceCapabilities;
            capabilities.NumberOfPhases = 2;

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            // Act
            var result = source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.AreEqual(SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES, result);
            Assert.AreEqual(null, currentLoadpoint);
        }
        #endregion

        #region CapabilityIssues
        [Test]
        public void TestTooHighVoltage()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            Loadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Voltages[0].Rms = 500;

            // Act
            var result = source.SetLoadpoint(lp);

            // Assert
            var nextLoadpoint = source.GetNextLoadpoint();

            Assert.AreEqual(SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_TOO_HIGH, result);
            Assert.AreEqual(null, nextLoadpoint);
        }

        [Test]
        public void TestTooHighCurrent()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            Loadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Currents[0].Rms = 100;

            // Act
            var result = source.SetLoadpoint(lp);

            // Assert
            var nextLoadpoint = source.GetNextLoadpoint();

            Assert.AreEqual(SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_TOO_HIGH, result);
            Assert.AreEqual(null, nextLoadpoint);
        }

        [Test]
        public void TestTooManyHarmonics()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            Loadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;

            for (int i = 0; i < 25; ++i)
            {
                lp.Voltages[0].Harmonics.Add(0.5);
            }

            // Act
            var result = source.SetLoadpoint(lp);

            // Assert
            var nextLoadpoint = source.GetNextLoadpoint();

            Assert.AreEqual(SourceResult.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS, result);
            Assert.AreEqual(null, nextLoadpoint);
        }
        #endregion

        #region HelperFunctions
        private static ISource GenerateSimulatedSource(Mock<ILogger<SimulatedSource>>? loggerMock = null, Mock<IConfiguration>? configMock = null, SourceCapabilities? capabilities = null)
        {
            loggerMock ??= new();
            configMock ??= new();

            if (capabilities == null)
                return new SimulatedSource(loggerMock.Object, configMock.Object);

            return new SimulatedSource(loggerMock.Object, configMock.Object, capabilities);
        }
    }
    #endregion
}
