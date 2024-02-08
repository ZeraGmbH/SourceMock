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
                var current = loadpoint.Phases[i].Current;

                if (current?.On != true) continue;

                var actualRms = current.Rms;
                var allowedRange = capabilities.Phases[i].Current;

                if (!allowedRange.IsIncluded(actualRms))
                    return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;

                var firstActive = loadpoint.Phases.FirstOrDefault(p => p.Current?.On == true);

                // IEC norm expects the first active current to be 0°
                if (firstActive != null && firstActive.Current.Angle != 0)
                    return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

                var isAngleValue = CheckAngle(current.Angle);
                if (isAngleValue != SourceApiErrorCodes.SUCCESS)
                    return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;
            }
            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckVoltages(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                // Is a current-only source
                if (capabilities.Phases[i].Voltage == null)
                    continue;

                var voltage = loadpoint.Phases[i].Voltage;

                if (voltage?.On != true) continue;

                var actualRms = voltage.Rms;
                var allowedRange = capabilities.Phases[i].Voltage;

                if (!allowedRange.IsIncluded(actualRms))
                    return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;

                var isAngleValue = CheckAngle(voltage.Angle);
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

