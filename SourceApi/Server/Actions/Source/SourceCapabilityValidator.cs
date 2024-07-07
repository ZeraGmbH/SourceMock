using SharedLibrary.DomainSpecific;
using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Verifies wheather or not a loadpoint is suitable for a given source.
    /// </summary>
    public class SourceCapabilityValidator : ISourceCapabilityValidator
    {
        public SourceApiErrorCodes IsValid(TargetLoadpoint loadpoint, SourceCapabilities capabilities)
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

        private static bool CheckNumberOfPhasesAreEqual(TargetLoadpoint loadpoint, SourceCapabilities capabilities)
            => loadpoint.Phases.Count != capabilities.Phases.Count;

        private static SourceApiErrorCodes CheckAngle(Angle actualAngle)
        {
            if (actualAngle < Angle.Zero || actualAngle >= new Angle(360))
                return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckCurrents(TargetLoadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                if (loadpoint.Phases[i].Current.On != true) continue;

                var acCurrent = loadpoint.Phases[i].Current.AcComponent;

                if (acCurrent != null)
                {
                    if (capabilities.Phases[i].AcCurrent == null)
                        return SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_AC;
                    return CheckAcCurrents(loadpoint, capabilities, i, acCurrent);
                }

                var dcCurrent = loadpoint.Phases[i].Current.DcComponent;

                if (dcCurrent != null)
                {
                    if (capabilities.Phases[i].DcCurrent == null)
                        return SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_DC;
                    return CheckDcCurrents(capabilities, i, dcCurrent.Value);
                }
            }

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckAcCurrents(TargetLoadpoint loadpoint, SourceCapabilities capabilities, int i, ElectricalVectorQuantity<Current> current)
        {
            var actualRms = current.Rms;
            var allowedRange = capabilities.Phases[i].AcCurrent!;

            if (!allowedRange.IsIncluded(actualRms))
                return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;

            var firstActive = loadpoint.Phases.FirstOrDefault(p => p.Current?.On == true);

            // IEC norm expects the first active current to be 0°
            if (firstActive != null && firstActive.Current.AcComponent!.Angle != Angle.Zero)
                return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            var isAngleValue = CheckAngle(current.Angle);
            if (isAngleValue != SourceApiErrorCodes.SUCCESS)
                return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckDcCurrents(SourceCapabilities capabilities, int i, Current actualRms)
        {
            var allowedRange = capabilities.Phases[i].DcCurrent!;

            if (!allowedRange.IsIncluded(actualRms))
                return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckVoltages(TargetLoadpoint loadpoint, SourceCapabilities capabilities)
        {
            for (int i = 0; i < loadpoint.Phases.Count; ++i)
            {
                if (loadpoint.Phases[i].Voltage?.On != true) continue;

                var acVoltage = loadpoint.Phases[i].Voltage.AcComponent;

                if (acVoltage != null)
                {
                    if (capabilities.Phases[i].AcVoltage == null)
                        return SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_AC;
                    return CheckAcVoltage(capabilities, i, acVoltage);
                }

                var dcVoltage = loadpoint.Phases[i].Voltage.DcComponent;

                if (dcVoltage != null)
                {
                    if (capabilities.Phases[i].DcVoltage == null)
                        return SourceApiErrorCodes.SOURCE_NOT_COMPATIBLE_TO_AC;
                    return CheckDcVoltage(capabilities, i, dcVoltage.Value);
                }
            }

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckAcVoltage(SourceCapabilities capabilities, int i, ElectricalVectorQuantity<Voltage> voltage)
        {
            var actualRms = voltage!.Rms;
            var allowedRange = capabilities.Phases[i].AcVoltage;

            if (!allowedRange!.IsIncluded(actualRms))
                return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;

            var isAngleValue = CheckAngle(voltage.Angle);
            if (isAngleValue != SourceApiErrorCodes.SUCCESS)
                return SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckDcVoltage(SourceCapabilities capabilities, int i, Voltage voltage)
        {
            var allowedRange = capabilities.Phases[i].DcVoltage;

            if (!allowedRange!.IsIncluded(voltage))
                return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID;

            return SourceApiErrorCodes.SUCCESS;
        }

        private static SourceApiErrorCodes CheckFrequencies(TargetLoadpoint loadpoint, SourceCapabilities capabilities)
        {
            if (loadpoint.Frequency.Mode != FrequencyMode.SYNTHETIC)
                return SourceApiErrorCodes.SUCCESS;

            var generatedFrequency = loadpoint.Frequency;

            if (capabilities.FrequencyRanges == null)
                return SourceApiErrorCodes.SUCCESS;

            foreach (var range in capabilities.FrequencyRanges)
                if (range.Mode == FrequencyMode.SYNTHETIC)
                    if (generatedFrequency.Value >= range.Min && generatedFrequency.Value <= range.Max)
                        return SourceApiErrorCodes.SUCCESS;

            return SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_FREQUENCY_INVALID;
        }
    }
}

