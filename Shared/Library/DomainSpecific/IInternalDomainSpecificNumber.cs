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