using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SourceApi.Actions.Source;
using SourceApi.Model;
using SourceApi.Tests.Actions.LoadpointValidator;

namespace SourceApi.Tests.Actions.Source
{
    internal class SimulatedSourceTests
    {

        const string CONFIG_KEY_NUMBER_OF_PHASES = "SourceProperties:NumberOfPhases";

        #region PositiveTestCases
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public async Task TestValidTurnOn(TargetLoadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.GetSourceCapabilitiesForNumberOfPhases(loadpoint.Phases.Count);

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            // Act
            var result = await source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));
            Assert.That(loadpoint, Is.EqualTo(currentLoadpoint));

            var info = source.GetActiveLoadpointInfo();
            Assert.That(info.IsActive, Is.EqualTo(true));
        }

        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public async Task TestValidTurnOff(TargetLoadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.GetSourceCapabilitiesForNumberOfPhases(loadpoint.Phases.Count);

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            await source.SetLoadpoint(loadpoint);

            // Act
            var result = await source.TurnOff();

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));

            var info = source.GetActiveLoadpointInfo();
            Assert.That(info.IsActive, Is.EqualTo(false));
        }
        #endregion

        #region LoadpointIssues
        [Test]
        [TestCaseSource(typeof(SimulatedSourceTestData), nameof(SimulatedSourceTestData.ValidLoadpointsWithOneOrThreePhases))]
        public async Task TestTurnOnWithInvalidLoadpoint(TargetLoadpoint loadpoint)
        {
            // Arrange
            var capabilities = SimulatedSourceTestData.DefaultTwoPhaseSourceCapabilities;

            ISource source = GenerateSimulatedSource(capabilities: capabilities);

            // Act
            var result = await source.SetLoadpoint(loadpoint);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES));
            Assert.That(currentLoadpoint, Is.Null);


            var info = source.GetActiveLoadpointInfo();
            Assert.That(info.IsActive, Is.EqualTo(null));
        }
        #endregion

        #region CapabilityIssues
        [Test]
        public async Task TestTooHighVoltage()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            TargetLoadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Phases[0].Voltage.AcComponent.Rms = 500;

            // Act
            var result = await source.SetLoadpoint(lp);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID));
            Assert.That(currentLoadpoint, Is.Null);
        }

        [Test]
        public async Task TestTooHighCurrent()
        {
            // Arrange 
            ISource source = GenerateSimulatedSource();
            TargetLoadpoint lp = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            lp.Phases[0].Current.AcComponent.Rms = 100;

            // Act
            var result = await source.SetLoadpoint(lp);

            // Assert
            var currentLoadpoint = source.GetCurrentLoadpoint();

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID));
            Assert.That(currentLoadpoint, Is.Null);
        }
        #endregion

        #region HelperFunctions
        private static ISource GenerateSimulatedSource(Mock<ILogger<SimulatedSource>>? loggerMock = null, Mock<IConfiguration>? configMock = null, SourceCapabilities? capabilities = null)
        {
            loggerMock ??= new();

            if (capabilities == null)
                return new SimulatedSource(loggerMock.Object, new SourceCapabilityValidator());

            return new SimulatedSource(loggerMock.Object, capabilities, new SourceCapabilityValidator());
        }
        #endregion

        [Test]
        public async Task Returns_Correct_Dosage_Progress()
        {
            Mock<ILogger<SimulatedSource>> logger = new();

            SimulatedSource mock = new(logger.Object, new SourceCapabilityValidator());

            await mock.SetLoadpoint(GetLoadpoint());
            await mock.SetDosageEnergy(20, 1);
            await mock.StartDosage();

            DosageProgress result = await mock.GetDosageProgress(1);

            Assert.That(result.Active, Is.EqualTo(true));
            Assert.That(result.Remaining, Is.LessThan(20));
            Assert.That(result.Progress, Is.GreaterThan(0));
        }

        [Test]
        public async Task Turns_Off_Loadpoint()
        {
            Mock<ILogger<SimulatedSource>> logger = new();

            SimulatedSource mock = new(logger.Object, new SourceCapabilityValidator());

            var loadpoint = GetLoadpoint();
            await mock.SetLoadpoint(loadpoint);

            await mock.TurnOff();

            foreach (var phase in loadpoint.Phases)
            {
                Assert.That(phase.Voltage.On, Is.EqualTo(true));
                Assert.That(phase.Current.On, Is.EqualTo(true));
            }

            var info = mock.GetActiveLoadpointInfo();

            Assert.That(info.IsActive, Is.EqualTo(false));
        }

        private static TargetLoadpoint GetLoadpoint()
        {
            return new()
            {
                Frequency = new() { Value = 50 },
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new(){AcComponent = new() {Angle=0, Rms=10}, On=true},
                        Voltage = new(){AcComponent = new() {Angle=0, Rms=220}, On=true}
                    },
                    new TargetLoadpointPhase(){
                        Current = new(){AcComponent = new() {Angle=120, Rms=10}, On=true},
                        Voltage = new(){AcComponent = new() {Angle=120, Rms=220}, On=true}
                    },
                    new TargetLoadpointPhase(){
                        Current = new(){AcComponent = new() {Angle=240, Rms=10}, On=true},
                        Voltage = new(){AcComponent = new() {Angle=240, Rms=220}, On=true}
                    }
                }
            };
        }
    }
}
