

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Tests.Actions.Source
{
    public class SourceCapabilityValidatorTest
    {
        private readonly SourceCapabilityValidator validator = new();

        [Test]
        public void Returns_Success_On_Valid_DC_Loadpoint()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetDcLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetDcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));
        }

        [Test]
        public void Returns_Too_High_Voltage_On_DC_Loadpoint()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetTooHighVoltageDcLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetDcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID));
        }

        [Test]
        public void Returns_Too_High_Current_On_DC_Loadpoint()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetTooHighCurrentDcLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetDcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID));
        }

        [Test]
        public void Returns_Source_Is_Not_Compatible_To_DC_If_DcCurrent_Is_Null()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetDcLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetAcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_DC));
        }

        [Test]
        public void Returns_Source_Is_Not_Compatible_To_DC_If_DcVoltage_Is_Null()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetDcLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetAcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_DC));
        }

        [Test]
        public void Returns_Source_Is_Not_Compatible_To_AC_If_AcCurrent_Is_Null()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetACLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetDcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_AC));
        }

        [Test]
        public void Returns_Source_Is_Not_Compatible_To_AC_If_AcVoltage_Is_Null()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetACLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetDcSourceCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_AC));
        }

        [Test]
        public void No_Validation_Is_Processed_If_All_Quantaties_Are_Off()
        {
            TargetLoadpoint loadpoint = TestLoadpoints.GetNullLoadpoint();
            SourceCapabilities capabilities = TestCapabilities.GetNullCapabilities();
            var result = validator.IsValid(loadpoint, capabilities);

            Assert.That(result, Is.EqualTo(SourceApiErrorCodes.SUCCESS));
        }
    }
}