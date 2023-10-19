using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.Source
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
                loadpoint.Phases.Count() != capabilities.Phases.Count;
        }

        private static SourceResult CheckCurrents(Loadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                if (!capabilities.Phases[i].Current.IsIncluded(loadpoint.Phases[i].Current.Rms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;
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

                if (!capabilities.Phases[i].Voltage.IsIncluded(loadpoint.Phases[i].Voltage.Rms))
                    return SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;

            }
            return SourceResult.SUCCESS;
        }
    }

}
