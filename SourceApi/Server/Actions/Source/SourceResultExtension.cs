namespace SourceApi.Actions.Source
{
    public static partial class SourceResultExtension
    {
        public static string ToUserFriendlyString(this SourceApiErrorCodes sourceResult)
        {
            switch (sourceResult)
            {
                case SourceApiErrorCodes.SUCCESS:
                    return "The operation was successful.";
                case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES:
                    return "The loadpoint that was tried to be set has a different number of phases than what this source can provide.";
                case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID:
                    return "The voltage which was tried to be set was higher that what this source can provide.";
                case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID:
                    return "The current that was tried to be set was higher than what this source can provide.";
                case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_FREQUENCY_INVALID:
                    return "The frequency that was tried to be set was not in the range what this source can provide.";
                case SourceApiErrorCodes.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS:
                    return "The number of harmonics given in the loadpoint is higher than what the source can provide.";
                case SourceApiErrorCodes.LOADPOINT_ANGLE_INVALID:
                    return "The angle must be between 0 (inclusive) and 360 (exclusive) degrees.";
                case SourceApiErrorCodes.SUCCESS_NOT_ACTIVATED:
                    return "The loadpoint is valid and has been saved but it could not be activated on the device.";
                default:
                    return "There is no user friendly explainaition for this error implemented.";
            }
        }
    }
}