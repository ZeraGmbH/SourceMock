using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Verifies wheather or not a loadpoint is suitable for a given source.
    /// </summary>
    public static class SourceCapabilityValidator
    {
        public static SourceApiErrorCodes IsValid(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (CheckNumberOfPhasesAreEqual(loadpoint, capabilities))
                return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES;

            var currentResult = CheckCurrents(loadpoint, capabilities);
            if (currentResult != SourceApiErrorCodes.SUCCESS)
                return currentResult;

            var voltageResult = CheckVoltages(loadpoint, capabilities);
            if (voltageResult != SourceApiErrorCodes.SUCCESS)
                return voltageResult;

            var frequencyResult = CheckFrequencies(loadpoint, capabilities);
            if (frequencyResult != SourceApiErrorCodes.SUCCESS)
                return frequencyResult;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static bool CheckNumberOfPhasesAreEqual(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            return
                loadpoint.Phases.Count() != capabilities.Phases.Count;
        }

        private static SourceApiErrorCodes CheckAngle(double actualAngle)
        {
            if (actualAngle < 0 || actualAngle >= 360)
                return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckCurrents(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                var actualRms = loadpoint.Phases[i].Current.Rms;
                var allowedRange = capabilities.Phases[i].Current;

                if (!allowedRange.IsIncluded(actualRms))
                    return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;

                var isAngleValue = CheckAngle(loadpoint.Phases[i].Current.Angle);
                if (isAngleValue != SourceApiErrorCodes.SUCCESS)
                    return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;
            }
            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckVoltages(Loadpoint loadpoint, SourceCapabilities capabilities)
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
                    return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;


                var isAngleValue = CheckAngle(loadpoint.Phases[i].Voltage.Angle);
                if (isAngleValue != SourceApiErrorCodes.SUCCESS)
                    return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            }
            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckFrequencies(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (loadpoint.Frequency.Mode != FrequencyMode.SYNTHETIC)
                return SourceApiErrorCodes.SUCCESS;

            var frequency = loadpoint.Frequency.Value;

            foreach (var range in capabilities.FrequencyRanges)
                if (range.Mode == FrequencyMode.SYNTHETIC)
                    if (frequency >= range.Min && frequency <= range.Max)
                        return SourceApiErrorCodes.SUCCESS;

            return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_FREQUENCY_INVALID;
        }
    }
}

