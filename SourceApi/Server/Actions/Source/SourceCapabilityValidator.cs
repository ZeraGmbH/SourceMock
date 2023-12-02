using SourceApi.Model;

namespace SourceApi.Actions.Source
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

            var frequencyResult = CheckFrequencies(loadpoint, capabilities);
            if (frequencyResult != SourceResult.SUCCESS)
                return frequencyResult;

            return SourceResult.SUCCESS;
        }

        private static bool CheckNumberOfPhasesAreEqual(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            return
                loadpoint.Phases.Count() != capabilities.Phases.Count;
        }

        private static SourceResult CheckAngle(double actualAngle)
        {
            if (actualAngle < 0 || actualAngle >= 360)
                return SourceResult.LOADPOINT_ANGLE_INVALID;

            return SourceResult.SUCCESS;
        }

        private static SourceResult CheckCurrents(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                var actualRms = loadpoint.Phases[i].Current.Rms;
                var allowedRange = capabilities.Phases[i].Current;

                if (!allowedRange.IsIncluded(actualRms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;

                var isAngleValue = CheckAngle(loadpoint.Phases[i].Current.Angle);
                if (isAngleValue != SourceResult.SUCCESS)
                    return SourceResult.LOADPOINT_ANGLE_INVALID;
            }
            return SourceResult.SUCCESS;
        }

        private static SourceResult CheckVoltages(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                if (capabilities.Phases[i].Voltage == null)
                {
                    // Is a current-only source
                    continue;
                }

                var actualRms = loadpoint.Phases[i].Voltage.Rms;
                var allowedRange = capabilities.Phases[i].Voltage;

                if (!allowedRange.IsIncluded(actualRms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;


                var isAngleValue = CheckAngle(loadpoint.Phases[i].Voltage.Angle);
                if (isAngleValue != SourceResult.SUCCESS)
                    return SourceResult.LOADPOINT_ANGLE_INVALID;

            }
            return SourceResult.SUCCESS;
        }

        private static SourceResult CheckFrequencies(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (loadpoint.Frequency.Mode != FrequencyMode.SYNTHETIC)
                return SourceResult.SUCCESS;

            var frequency = loadpoint.Frequency.Value;

            foreach (var range in capabilities.FrequencyRanges)
                if (range.Mode == FrequencyMode.SYNTHETIC)
                    if (frequency >= range.Min && frequency <= range.Max)
                        return SourceResult.SUCCESS;

            return SourceResult.LOADPOINT_NOT_SUITABLE_FREQUENCY_INVALID;
        }
    }
}

