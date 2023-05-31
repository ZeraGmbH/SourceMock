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
            Loadpoint loadpoint1 = new Loadpoint()
            {
                PhaseAngleVoltage = 180
            };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(0, errCount);
        }


        [Test]
        public void TestInvalidPhaseAngleVoltageTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint()
            {
                PhaseAngleVoltage = -1
            };

            // Act
            var errCount = ValidateObject(loadpoint1);
   
            // Assert
            Assert.AreEqual(1, errCount);
        }

        [Test]
        public void TestInvalidPhaseAngleVoltageTooHigh()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint()
            {
                PhaseAngleVoltage = 361
            };

            // Act
            var errCount = ValidateObject(loadpoint1);

            // Assert
            Assert.AreEqual(1, errCount);
        }

        private int ValidateObject(object obj)
        {
            var validationContext = new ValidationContext(obj, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(obj, validationContext, validationResults, true);
            return validationResults.Count;
        }
    }
}
