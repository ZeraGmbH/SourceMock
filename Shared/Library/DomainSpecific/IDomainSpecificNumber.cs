namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public interface IDomainSpecificNumber
{
    /// <summary>
    /// The natural unit of the number.
    /// </summary>
    string Unit { get; }
}

/// <summary>
/// Base class for all domain specific numbers.
/// </summary>
public interface IDomainSpecificNumber<T> : IDomainSpecificNumber, IComparable<T> where T : IDomainSpecificNumber<T>
{
    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="value">Value to use.</param>
    /// <returns>New instance.</returns>
    static abstract T Create(double value);

    /// <summary>
    /// Extract the underlying number.
    /// </summary>
    /// <param name="value">The domain specific number.</param>
    /// <returns>The real value.</returns>
    static abstract explicit operator double(T value);

    /// <summary>
    /// Add two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Sum of numbers.</returns>
    static abstract T operator +(T left, T right);

    /// <summary>
    /// Subtract two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Difference of numbers.</returns>
    static abstract T operator -(T left, T right);

    /// <summary>
    /// Negate a domain specific number.
    /// </summary>
    /// <param name="value">Domain specific number.</param>
    /// <returns>Negated number.</returns>
    static abstract T operator -(T value);

    /// <summary>
    /// Get reminder of domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Reminder of numbers.</returns>
    static abstract T operator %(T left, T right);

    /// <summary>
    /// Multiply a domain specific number with a factor.
    /// </summary>
    /// <param name="value">Domain specific number.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled number.</returns>
    static abstract T operator *(T value, double factor);

    /// <summary>
    /// Divide a domain specific number by a factor.
    /// </summary>
    /// <param name="value">Domain specific number.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled number.</returns>
    static abstract T operator /(T value, double factor);

    /// <summary>
    /// Multiply a domain specific number with a factor.
    /// </summary>
    /// <param name="value">Domain specific number.</param>
    /// <param name="factor">Scaling factor.</param>
    /// <returns>Scaled number.</returns>
    static abstract T operator *(double factor, T value);

    /// <summary>
    /// Devide two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Ratio of numbers.</returns>
    static abstract double operator /(T left, T right);

    /// <summary>
    /// Test a domain specific number.
    /// </summary>
    /// <param name="value">Domain specific number.</param>
    /// <returns>Set if number is exactly zero.</returns>
    static abstract bool operator !(T value);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if numbers are exactly equal.</returns>    
    static abstract bool operator ==(T left, T right);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if numbers are not exactly equal.</returns>    
    static abstract bool operator !=(T left, T right);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if first number is less than second number.</returns>    
    static abstract bool operator <(T left, T right);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if first number is not greater than second number.</returns>    
    static abstract bool operator <=(T left, T right);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if first number is greater than second number.</returns>    
    static abstract bool operator >(T left, T right);

    /// <summary>
    /// Compare two domain specific numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if first number is not less than second number.</returns>    
    static abstract bool operator >=(T left, T right);

}
