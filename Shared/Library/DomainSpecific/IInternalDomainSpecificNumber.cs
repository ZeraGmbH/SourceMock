namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Operations only available inside the shared library
/// </summary>
internal interface IInternalDomainSpecificNumber : IDomainSpecificNumber
{
    /// <summary>
    /// Get the current value, e.g. for formatting.
    /// </summary>
    double GetValue();
}

/// <summary>
/// Operations only available inside the shared library
/// </summary>
internal interface IInternalDomainSpecificNumber<T> : IInternalDomainSpecificNumber, IDomainSpecificNumber<T> where T : IInternalDomainSpecificNumber<T>
{
}

/// <summary>
/// Helper methods on domain specific numbers.
/// </summary>
public static class IDomainSpecificNumberExtensions
{
    /// <summary>
    /// Format a number.
    /// </summary>
    /// <param name="number">Number to format.</param>
    /// <param name="provider">Regional settings to use.</param>
    /// <param name="withUnit">Add the unit to the number.</param>
    /// <param name="spaceUnit">Separate unit from number by a single space.</param>
    /// <returns>String as requested.</returns>
    public static string? Format(this IDomainSpecificNumber number, IFormatProvider? provider, bool withUnit = true, bool spaceUnit = false)
        => number.Format(null, provider, withUnit, spaceUnit);

    /// <summary>
    /// Allow direct access to the value - may be necessary in dynamic scenarios.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>The raw value.</returns>
    public static double GetValue(this IDomainSpecificNumber number) => ((IInternalDomainSpecificNumber)number).GetValue();

    /// <summary>
    /// Get the absolute value of a domain specific number.
    /// </summary>
    /// <param name="value">The number.</param>
    /// <typeparam name="T">Implementation type of the number.</typeparam>
    /// <returns>A new number with the absolute value.</returns>
    public static T Abs<T>(this T value) where T : IDomainSpecificNumber<T> => T.Create(Math.Abs((double)value));

    /// <summary>
    /// Get the smaller value.
    /// </summary>
    /// <param name="value">A domain specific number.</param>
    /// <param name="other">Another domain specific number.</param>
    /// <typeparam name="T">Implementation type of both numbers.</typeparam>
    /// <returns>First parameter if first value is less or equal to the second value.</returns>
    public static T Smallest<T>(this T value, T other) where T : IDomainSpecificNumber<T> => (double)value <= (double)other ? value : other;

    /// <summary>
    /// Get the larger value.
    /// </summary>
    /// <param name="value">A domain specific number.</param>
    /// <param name="other">Another domain specific number.</param>
    /// <typeparam name="T">Implementation type of both numbers.</typeparam>
    /// <returns>First parameter if first value is greater or equal to the second value.</returns>
    public static T Largest<T>(this T value, T other) where T : IDomainSpecificNumber<T> => (double)value >= (double)other ? value : other;

    /// <summary>
    /// Check if a domain specifc number is NaN.
    /// </summary>
    /// <param name="value">Some number.</param>
    /// <typeparam name="T">Implementation type.</typeparam>
    /// <returns>Set if the number is NaN.</returns>
    public static bool IsNaN<T>(this T value) where T : IDomainSpecificNumber<T> => double.IsNaN((double)value);

    /// <summary>
    /// Format a number.
    /// </summary>
    /// <param name="number">Number to format.</param>
    /// <param name="format">Format to use.</param>
    /// <param name="provider">Regional settings to use.</param>
    /// <param name="withUnit">Add the unit to the number.</param>
    /// <param name="spaceUnit">Separate unit from number by a single space.</param>
    /// <returns>String as requested.</returns>
    public static string? Format(this IDomainSpecificNumber number, string? format = null, IFormatProvider? provider = null, bool withUnit = true, bool spaceUnit = false)
    {
        var formatted = ((IInternalDomainSpecificNumber)number).GetValue().ToString(format, provider);

        return withUnit ? $"{formatted}{(spaceUnit ? " " : "")}{number.Unit}" : formatted;
    }
}