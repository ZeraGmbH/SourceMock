using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Rective power (in VAr) as domain specific number.
/// </summary>
public readonly struct ReactivePower(double value) : IInternalDomainSpecificNumber<ReactivePower>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VAr";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ReactivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ReactivePower power) => power._Value;

    /// <summary>
    /// Add to reactive power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New reactive power instance representing the sum of the parameters.</returns>
    public static ReactivePower operator +(ReactivePower left, ReactivePower right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ReactivePower operator *(ReactivePower power, double factor) => new(power._Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ReactivePower operator *(double factor, ReactivePower power) => new(factor * power._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double operator /(ReactivePower left, ReactivePower right) => left._Value / right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ReactivePower operator -(ReactivePower left, ReactivePower right) => new(left._Value - right._Value);


    #region Comparable

    /// <summary>
    /// See if the number is exactly zero.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>Set if number is zero.</returns>
    public static bool operator !(ReactivePower number) => number._Value == 0;

    /// <summary>
    /// Compare with any other object.
    /// </summary>
    /// <param name="obj">Some other object.</param>
    /// <returns>Set if this number is identical to the other object.</returns>
    public override bool Equals(object? obj) => obj is ReactivePower angle && _Value == angle._Value;

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
    public int CompareTo(ReactivePower other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are exactly identical.</returns>
    public static bool operator ==(ReactivePower left, ReactivePower right) => left._Value == right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are not exactly identical.</returns>
    public static bool operator !=(ReactivePower left, ReactivePower right) => left._Value != right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is less than the second number.</returns>
    public static bool operator <(ReactivePower left, ReactivePower right) => left._Value < right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not greater than the second number.</returns>
    public static bool operator <=(ReactivePower left, ReactivePower right) => left._Value <= right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is greater than the second number.</returns>
    public static bool operator >(ReactivePower left, ReactivePower right) => left._Value > right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not less than the second number.</returns>
    public static bool operator >=(ReactivePower left, ReactivePower right) => left._Value >= right._Value;

    #endregion
}
