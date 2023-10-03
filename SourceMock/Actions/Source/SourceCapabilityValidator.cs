using SourceMock.Model;

namespace SourceMock.Actions.Source
{
    /// <summary>
    /// Verifies wheather or not a loadpoint is suitable for a given source.
    /// </summary>
    public static class SourceCapabilityValidator
    {
        public static SourceResult IsValid(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (CheckNumberOfPhasesAreEqual(loadpoint, capabilities))
                return SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES;

            var currentResult = CheckCurrents(loadpoint, capabilities);
            if (currentResult != SourceResult.SUCCESS)
                return currentResult;

            var voltageResult = CheckVoltages(loadpoint, capabilities);
            if (voltageResult != SourceResult.SUCCESS)
                return voltageResult;

            return SourceResult.SUCCESS;
        }

        private static bool CheckNumberOfPhasesAreEqual(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            return
                loadpoint.Currents.Count() != capabilities.NumberOfPhases ||
                loadpoint.Voltages.Count() != capabilities.NumberOfPhases;
        }

        private static SourceResult CheckCurrents(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            foreach (var current in loadpoint.Currents)
            {
                if (!capabilities.CurrentRanges.IsIncluded(current.Rms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_TOO_HIGH;

                if (current.Harmonics.Count() > capabilities.MaxHarmonic + 2)
                {
                    return SourceResult.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS;
                }
            }
            return SourceResult.SUCCESS;
        }

        private static SourceResult CheckVoltages(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (capabilities.VoltageRanges == null || capabilities.VoltageRanges.Count == 0)
            {
                // Is a current-only source
                return SourceResult.SUCCESS;
            }

            foreach (var voltage in loadpoint.Voltages)
            {
                if (!capabilities.VoltageRanges.IsIncluded(voltage.Rms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_TOO_HIGH;

                if (voltage.Harmonics.Count() > capabilities.MaxHarmonic + 2)
                {
                    return SourceResult.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS;
                }
            }
            return SourceResult.SUCCESS;
        }
    }

}
