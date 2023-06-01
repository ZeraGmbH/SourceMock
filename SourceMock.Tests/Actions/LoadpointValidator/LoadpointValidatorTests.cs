using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization.Formatters.Binary;
using SourceMock.Actions.LoadpointValidator;
using SourceMock.Model;
using SourceMock.Extensions;


namespace SourceMock.Tests.Actions.LoadpointValidator
{
    internal class LoadpointValidatorTests
    {
        #region TestPhaseAngleVoltageValidation
        [Test]
        public void TestValidPhaseAngleVoltage()
        {
            // Arrange
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
            loadpoint1.PhaseAnglesVoltage = new[] { 0d, 0d, 180d };            

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(0, errCount);
        }


        [Test]
        public void TestInvalidPhaseAngleVoltageTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
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
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
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
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
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
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
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
            Loadpoint loadpoint1 = LoadpointValidatorTestData.loadpoint001.Copy();
            loadpoint1.PhaseAnglesCurrent = new[] { 0d, 0d, 360.1d };

            loadpoint1.PhaseAnglesCurrent = loadpoint1.PhaseAnglesCurrent.Append(360.1);

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        /// <summary>
        /// Validates an object against the restrictions set in its annotations.
        /// </summary>
        /// <param name="obj">The object to validate.</param>
        /// <returns>The number of errors in the object, therefore zero if valid.</returns>
        private int ValidateObject(object obj)
        {
            var validationContext = new ValidationContext(obj, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return validationResults.Count;
        }
        #endregion
    }
}
