using SourceMock.Model;

namespace SourceMock.Actions.LoadpointValidator
{
    /// <summary>
    /// Provides a function to validate Loadpoint objects.
    /// </summary>
    public static class LoadpointValidator
    {
        /// <summary>
        /// A Set of possible results of the dynamic validation of a loadpoint object.
        /// </summary>
        public enum ValidationResult
        {
            /// <summary>
            /// No problem was found with this loadpoint
            /// </summary>
            OK,
            /// <summary>
            /// The number of phases was different in at least one phase-specific field.
            /// </summary>
            NUMBER_OF_PHASES_DO_NOT_MATCH
        }

        /// <summary>
        /// Conducts the dynamic validation of a loadpoint object.
        /// </summary>
        /// <param name="loadpoint">The loadpoint to be validated.</param>
        /// <returns>The result of the validation.</returns>
        public static ValidationResult Validate(Loadpoint loadpoint)
        {
            return !CheckNumberOfPhases(loadpoint)
                ? ValidationResult.NUMBER_OF_PHASES_DO_NOT_MATCH
                : ValidationResult.OK;
        }

        private static bool CheckNumberOfPhases(Loadpoint loadpoint)
        {
            var phaseSpecificFields = new[] { loadpoint.Voltages, loadpoint.Currents };

            var numberOfPhases = phaseSpecificFields.First().Count;
            foreach (var phaseSpecificField in phaseSpecificFields)
            {
                if (phaseSpecificField.Count != numberOfPhases) return false;
            }

            return true;
        }
    }
}
