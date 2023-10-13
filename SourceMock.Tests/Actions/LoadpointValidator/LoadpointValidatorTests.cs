using System.ComponentModel.DataAnnotations;

using SourceMock.Model;

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

            // Assert
            Assert.AreEqual(0, errCount);
        }
        #endregion

        #region TestPhaseAngleValidation
        [Test]
        public void TestValidPhaseAngle()
        {
            // Arrange       
            ElectricalVectorQuantity electricalVectorQuantity = new()
            {
                Rms = 0d,
                Angle = 180d,
                On = true
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.AreEqual(0, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleTooLow()
        {
            // Arrange       
            ElectricalVectorQuantity electricalVectorQuantity = new()
            {
                Rms = 0d,
                Angle = -0.1d,
                On = true
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleTooHigh()
        {
            // Arrange       
            ElectricalVectorQuantity electricalVectorQuantity = new()
            {
                Rms = 0d,
                Angle = 360.1d,
                On = true
            };

            // Act
            var errCount = ValidateObject(electricalVectorQuantity);

            // Assert
            Assert.AreEqual(1, errCount);
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
