using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SourceApi.Actions.LoadpointValidator
{
    /// <summary>
    /// Specicifies the numeric range constraints for the values in an <see cref="IEnumerable"/>.
    /// </summary>
    public class RangeEnumerableAttribute : RangeAttribute
    {
        #region InheritedConstructors
        /// <inheritdoc/>
        public RangeEnumerableAttribute(double minimum, double maximum) : base(minimum, maximum)
        {
        }

        /// <inheritdoc/>
        public RangeEnumerableAttribute(int minimum, int maximum) : base(minimum, maximum)
        {
        }

        /// <inheritdoc/>
        public RangeEnumerableAttribute([DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type type, string minimum, string maximum) : base(type, minimum, maximum)
        {
        }
        #endregion

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
