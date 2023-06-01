using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

namespace SourceMock.Actions.LoadpointValidator
{
    public class RangeEnumerable : RangeAttribute
    {
        /// <inheritdoc/>
        public RangeEnumerable(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        /// <inheritdoc/>
        public RangeEnumerable(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        /// <inheritdoc/>
        public RangeEnumerable([DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }

        /// <inheritdoc/>
        public override bool IsValid(object? value)
        {
            if (null == value) { return true; } 

            IEnumerable<object> list = ((IEnumerable)value).Cast<object>();
            
            foreach (object item in list)
            {
                if (!base.IsValid(item))
                {
                    return false;
                }                
            }

            return true;
        }
    }
}
