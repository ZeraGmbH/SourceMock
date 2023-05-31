using SourceMock.Model;

namespace SourceMock.Actions.LoadpointValidator
{
    public static class LoadpointValidator
    {
        public enum ValidationResult
        {
            OK
        }
        
        public static ValidationResult Validate(Loadpoint loadpoint)
        {


            return ValidationResult.OK;
        }
    }
}
