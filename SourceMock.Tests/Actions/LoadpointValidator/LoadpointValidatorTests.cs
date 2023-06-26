using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using SourceMock.Actions.LoadpointValidator;
using SourceMock.Model;
using NUnit.Framework;
using System.Runtime.CompilerServices;

namespace SourceMock.Tests.Actions.LoadpointValidator
{
    internal class LoadpointValidatorTests
    {
        #region TestValidLoadpoints
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.ValidLoadpoints))]
        public void TestValidLoadpoints(Loadpoint loadpoint)
        {
            // Arrange
            // loadpoint set in parameter

            // Act
            var errCount = ValidateObject(loadpoint);
            var validationResult = SourceMock.Actions.LoadpointValidator.LoadpointValidator.Validate(loadpoint);

            // Assert
            Assert.AreEqual(0, errCount);
            Assert.AreEqual(
                SourceMock.Actions.LoadpointValidator.LoadpointValidator.ValidationResult.OK,
                validationResult);
        }
        #endregion

        #region TestPhaseAngleVoltageValidation
        [Test]
        public void TestInvalidPhaseAngleVoltageTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            loadpoint1.PhaseAnglesVoltage = new[] { 0d, 0d, -0.1d };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleVoltageTooHigh()
        {
            // Arrange       
            Loadpoint loadpoint1 = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            loadpoint1.PhaseAnglesVoltage = new[] { 0d, 0d, 360.1d };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }
        #endregion

        #region TestPhaseAngleCurrentValidation
        [Test]
        public void TestValidPhaseAngleCurrent()
        {
            // Arrange
            Loadpoint loadpoint1 = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            loadpoint1.PhaseAnglesCurrent = new[] { 0d, 0d, 180d };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(0, errCount);
        }


        [Test]
        public void TestInvalidPhaseAngleCurrentTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            loadpoint1.PhaseAnglesCurrent = new[] { 0d, 0d, -0.1d };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleCurrentTooHigh()
        {
            // Arrange       
            Loadpoint loadpoint1 = LoadpointValidatorTestData.Loadpoint001_3AC_valid;
            loadpoint1.PhaseAnglesCurrent = new[] { 0d, 0d, 360.1d };

            loadpoint1.PhaseAnglesCurrent = loadpoint1.PhaseAnglesCurrent.Append(360.1);

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }
        #endregion

        #region TestSameAmountOfPhases
        [Test]
        [TestCaseSource(typeof(LoadpointValidatorTestData), nameof(LoadpointValidatorTestData.InvalidLoadPoints_MissingPhase))]
        public void TestDifferentNumberOfPhases(Loadpoint loadpoint)
        {
            // Arrange
            // loadpoint set in parameter

            // Act
            var actual = SourceMock.Actions.LoadpointValidator.LoadpointValidator.Validate(loadpoint);

            // Assert
            Assert.AreEqual(
                SourceMock.Actions.LoadpointValidator.LoadpointValidator.ValidationResult.NUMBER_OF_PHASES_DO_NOT_MATCH,
                actual);
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
