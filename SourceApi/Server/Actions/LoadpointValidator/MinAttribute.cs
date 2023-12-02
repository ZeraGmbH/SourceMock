using System.Collections;

namespace SourceApi.Actions.LoadpointValidator
{
    /// <summary>
    /// Specicifies the numeric range constraints for the values in an <see cref="IEnumerable"/>.
    /// </summary>
    public class MinAttribute : Attribute
    {
        private readonly double _min;

        public MinAttribute(double minimum)
        {
            _min = minimum;
        }

        public MinAttribute(int minimum)
        {
            _min = minimum;
        }

        public bool IsValid(object? value)
        {
            if (value is double)
                return (double)value >= _min;

            if (value is int)
                return (int)value >= _min;

            return false;
        }
    }
}
