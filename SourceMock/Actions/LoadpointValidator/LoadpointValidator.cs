using SourceMock.Model;

namespace SourceMock.Actions.LoadpointValidator
{
    public static class LoadpointValidator
    {
        public enum ValidationResult
        {
            OK,
            NUMBER_OF_PHASES_DO_NOT_MATCH
        }
        
        public static ValidationResult Validate(Loadpoint loadpoint)
        {
            if (!CheckNumberOfPhases(loadpoint)) return ValidationResult.NUMBER_OF_PHASES_DO_NOT_MATCH;

            return ValidationResult.OK;
        }

        private static bool CheckNumberOfPhases(Loadpoint loadpoint)
        {
            var phaseSpecificFields = new[] {loadpoint.Voltages, loadpoint.Currents, loadpoint.PhaseAnglesVoltage, loadpoint.PhaseAnglesCurrent };
            
            var numberOfPhases = phaseSpecificFields.First().Count();
            foreach (var phaseSpecificField in phaseSpecificFields)
            {
                if (phaseSpecificField.Count() != numberOfPhases) return false;
            }

            return true;
        }
    }
}
