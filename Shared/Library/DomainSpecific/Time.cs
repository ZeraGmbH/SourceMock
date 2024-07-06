using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Time (in s) as domain specific number.
/// </summary>
public readonly struct Time(double value) : IInternalDomainSpecificNumber<Time>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "s";

    /// <summary>
    /// No time at all.
    /// </summary>
    public static readonly Time Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="time">Some active time.</param>
    public static explicit operator double(Time time) => time._Value;

    /// <summary>
    /// Add to time.
    /// </summary>
    /// <param name="left">One time.</param>
    /// <param name="right">Another time.</param>
    /// <returns>New time instance representing the sum of the parameters.</returns>
    public static Time operator +(Time left, Time right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale time by a factor.
    /// </summary>
    /// <param name="time">Some time.</param>
    /// <param name="factor">Factor to apply to the time.</param>
    /// <returns>New time with scaled value.</returns>
    public static Time operator *(Time time, double factor) => new(time._Value * factor);

    /// <summary>
    /// Scale time by a factor.
    /// </summary>
    /// <param name="time">Some time.</param>
    /// <param name="factor">Factor to apply to the time.</param>
    /// <returns>New time with scaled value.</returns>
    public static Time operator *(double factor, Time time) => new(factor * time._Value);

    #region Comparable

    /// <summary>
    /// See if the number is exactly zero.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>Set if number is zero.</returns>
    public static bool operator !(Time number) => number._Value == 0;

    /// <summary>
    /// Compare with any other object.
    /// </summary>
    /// <param name="obj">Some other object.</param>
    /// <returns>Set if this number is identical to the other object.</returns>
    public override bool Equals(object? obj) => obj is Time angle && _Value == angle._Value;

    /// <summary>
    /// Get a hashcode.
    /// </summary>
    /// <returns>Hashcode for this number.</returns>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <summary>
    /// Compare to another number.
    /// </summary>
    /// <param name="other">The other number.</param>
    /// <returns>Comparision result of the number.</returns>
    public int CompareTo(Time other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are exactly identical.</returns>
    public static bool operator ==(Time left, Time right) => left._Value == right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are not exactly identical.</returns>
    public static bool operator !=(Time left, Time right) => left._Value != right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is less than the second number.</returns>
    public static bool operator <(Time left, Time right) => left._Value < right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not greater than the second number.</returns>
    public static bool operator <=(Time left, Time right) => left._Value <= right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is greater than the second number.</returns>
    public static bool operator >(Time left, Time right) => left._Value > right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not less than the second number.</returns>
    public static bool operator >=(Time left, Time right) => left._Value >= right._Value;

    #endregion
}
