using System.ComponentModel.DataAnnotations;
using ZERA.WebSam.Shared.DomainSpecific;
using SourceApi.Actions.Source;
using SourceApi.Model;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models;

namespace SourceApi.Tests.Actions.LoadpointValidator
{
    internal class LoadpointValidatorTests
    {
        #region TestValidLoadpoints
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public void TestValidLoadpoints(TargetLoadpoint loadpoint)
        {
            // Arrange
            // loadpoint set in parameter

            // Act
            var errCount = ValidateObject(loadpoint);

            // Assert
            Assert.That(errCount, Is.Zero);
        }

        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.InvalidLoadpoints))]
        public void TestInvalidLoadpoints(TargetLoadpoint loadpoint)
        {
            // Arrange
            // loadpoint set in parameter
            var validator = new SourceCapabilityValidator();
            // Act
            var error = validator.IsValid(loadpoint, new()
            {
                FrequencyRanges = new() { new() { Max = new(100), Min = new(0), PrecisionStepSize = new(1) } },
                Phases = loadpoint.Phases
                    .Select(
                        p => new PhaseCapability
                        {
                            AcCurrent = new() { Min = new(0), Max = new(100), PrecisionStepSize = new(1) },
                            AcVoltage = new() { Min = new(0), Max = new(1000), PrecisionStepSize = new(1) }
                        }
                    ).ToList()
            });

            // Assert
            Assert.That(error, Is.EqualTo(SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID));
        }
        #endregion

        #region TestPhaseAngleValidation
        [Test]
        public void TestValidPhaseAngle()
        {
            // Arrange       
            ElectricalVectorQuantity<Voltage> electricalVectorQuantity = new()
            {
                Rms = new(0d),
                Angle = new(180d),
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.That(errCount, Is.Zero);
        }

        [Test]
        public void TestInvalidPhaseAngleTooLow()
        {
            // Arrange       
            ElectricalVectorQuantity<Voltage> electricalVectorQuantity = new()
            {
                Rms = new(0d),
                Angle = new(-0.1d),
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.That(errCount, Is.EqualTo(1));
        }

        [Test]
        public void TestInvalidPhaseAngleTooHigh()
        {
            // Arrange       
            ElectricalVectorQuantity<Voltage> electricalVectorQuantity = new()
            {
                Rms = new(0d),
                Angle = new(360.1d),
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.That(errCount, Is.EqualTo(1));
        }

        #endregion

        /// <summary>
        /// Validates an object against the restrictions set in its annotations.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>The number of errors in the object, therefore zero if valid.</returns>
        private static int ValidateObject(object obj)
        {
            var validationContext = new ValidationContext(obj, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return validationResults.Count;
        }
    }
}
