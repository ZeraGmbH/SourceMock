using System.ComponentModel.DataAnnotations;
using SourceMock.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceMock.Controllers;

namespace SourceMock.Tests.Actions.LoadpointValidator
{
    internal class LoadpointValidatorTests
    {
        [Test]
        public void TestValidPhaseAngleVoltage()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
            loadpoint1.PhaseAnglesVoltage = loadpoint1.PhaseAnglesVoltage.Append(180d);
            

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(0, errCount);
        }


        [Test]
        public void TestInvalidPhaseAngleVoltageTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
            loadpoint1.PhaseAnglesVoltage = loadpoint1.PhaseAnglesVoltage.Append(-0.1);

            // Act
            var errCount = ValidateObject(loadpoint1);
   
            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleVoltageTooHigh()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
            loadpoint1.PhaseAnglesVoltage = loadpoint1.PhaseAnglesVoltage.Append(360.1);

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestValidPhaseAngleCurrent()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
            loadpoint1.PhaseAnglesCurrent = loadpoint1.PhaseAnglesCurrent.Append(180d);


            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(0, errCount);
        }


        [Test]
        public void TestInvalidPhaseAngleCurrentTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
            loadpoint1.PhaseAnglesCurrent = loadpoint1.PhaseAnglesCurrent.Append(-0.1);

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleCurrentTooHigh()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint();
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
    }
}
