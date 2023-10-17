namespace SourceMock.Actions.Source
{
    public static partial class SourceResultExtension
    {
        public static string ToUserFriendlyString(this SourceResult sourceResult)
        {
            switch (sourceResult)
            {
                case SourceResult.SUCCESS:
                    return "The operation was successful.";
                case SourceResult.LOADPOINT_NOT_SUITABLE_DIFFERENT_NUMBER_OF_PHASES:
                    return "The loadpoint that was tried to be set has a different number of phases than what this source can provide.";
                case SourceResult.LOADPOINT_NOT_SUITABLE_VOLTAGE_INVALID:
                    return "The voltage which was tried to be set was higher that what this source can provide.";
                case SourceResult.LOADPOINT_NOT_SUITABLE_CURRENT_INVALID:
                    return "The current that was tried to be set was higher than what this source can provide.";
                case SourceResult.LOADPOINT_NOT_SUITABLE_TOO_MANY_HARMONICS:
                    return "The number of harmonics given in the loadpoint is higher than what the source can provide.";
                default:
                    return "There is no user friendly explainaition for this error implemented.";
            }
        }
    }
}