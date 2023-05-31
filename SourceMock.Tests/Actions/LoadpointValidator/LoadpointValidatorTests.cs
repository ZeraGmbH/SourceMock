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
        public void TestInvalidPhaseAngleVoltageTooLow()
        {
            // Arrange       
            Loadpoint loadpoint1 = new Loadpoint()
            {
                PhaseAngleVoltage = -1
            };

            // Act
            var validationContext = new ValidationContext(loadpoint1, null, null);
            var validationResults = new List<ValidationResult>();
            bool actual = Validator.TryValidateObject(loadpoint1, validationContext, validationResults, true);
   
            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(1, validationResults.Count);
        }
    }
}
