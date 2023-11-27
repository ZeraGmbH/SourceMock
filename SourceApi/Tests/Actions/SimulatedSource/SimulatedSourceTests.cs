
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using WebSamDeviceApis.Actions.Source;
using WebSamDeviceApis.Model;
using WebSamDeviceApis.Tests.Actions.LoadpointValidator;

namespace WebSamDeviceApis.Tests.Actions.Source
{
    internal class SimulatedSourceTests
    {
        const string CONFIG_KEY_NUMBER_OF_PHASES = "SourceProperties:NumberOfPhases";

        #region PositiveTestCases
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public async Task TestValidTurnOn(Loadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.GetSourceCapabilitiesForNumberOfPhases(loadpoint.Phases.Count);

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            // Act
            var result = await source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));
            Assert.That(loadpoint, Is.EqualTo(currentLoadpoint));
        }

        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public async Task TestValidTurnOff(Loadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.GetSourceCapabilitiesForNumberOfPhases(loadpoint.Phases.Count);

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            await source.SetLoadpoint(loadpoint);

            // Act
            var result = await source.TurnOff();

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceResult.SUCCESS));
            Assert.That(currentLoadpoint, Is.Null);
        }
        #endregion

        #region LoadpointIssues
        [Test]
        [TestCaseSource(typeof(SimulatedSourceTestData), nameof(SimulatedSourceTestData.ValidLoadpointsWithOneOrThreePhases))]
        public async Task TestTurnOnWithInvalidLoadpoint(Loadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.DefaultTwoPhaseSourceCapabilities;

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            // Act
            var result = await source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES));
            Assert.That(currentLoadpoint, Is.Null);
        }
        #endregion

        #region CapabilityIssues
        [Test]
        public async Task TestTooHighVoltage()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            Loadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Phases[0].Voltage.Rms = 500;

            // Act
            var result = await source.SetLoadpoint(lp);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID));
            Assert.That(currentLoadpoint, Is.Null);
        }

        [Test]
        public async Task TestTooHighCurrent()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            Loadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Phases[0].Current.Rms = 100;

            // Act
            var result = await source.SetLoadpoint(lp);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID));
            Assert.That(currentLoadpoint, Is.Null);
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
