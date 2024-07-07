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
    public static Time Create(double value) => new(value);

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

    /// <inheritdoc/>
    public override string? ToString() => this.Format();

    #region Arithmetics

    /// <inheritdoc/>
    public static Time operator +(Time left, Time right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Time operator -(Time left, Time right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Time operator -(Time value) => new(-value._Value);

    /// <inheritdoc/>
    public static Time operator %(Time left, Time right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Time operator *(Time value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static Time operator /(Time value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Time operator *(double factor, Time value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(Time left, Time right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Time number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Time angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Time other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Time left, Time right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Time left, Time right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Time left, Time right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Time left, Time right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Time left, Time right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Time left, Time right) => left._Value >= right._Value;

    #endregion
}
