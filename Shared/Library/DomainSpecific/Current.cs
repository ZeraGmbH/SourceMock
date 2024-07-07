using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Current (A) as domain specific number.
/// </summary>
public readonly struct Current(double value) : IInternalDomainSpecificNumber<Current>
{
    /// <summary>
    /// Create current 0.
    /// </summary>
    public Current() : this(0) { }

    /// <summary>
    /// No current at all.
    /// </summary>
    public static readonly Current Zero = new();

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static Current Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "A";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    public static explicit operator double(Current Current) => Current._Value;

    /// <inheritdoc/>
    public override string? ToString() => this.Format();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="voltage"></param>
    /// <returns></returns>
    public static ApparentPower operator *(Current current, Voltage voltage) => new(current._Value * (double)voltage);

    #region Arithmetics

    /// <inheritdoc/>
    public static Current operator +(Current left, Current right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Current operator -(Current left, Current right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Current operator -(Current value) => new(-value._Value);

    /// <inheritdoc/>
    public static Current operator %(Current left, Current right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Current operator /(Current value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Current operator *(Current value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static Current operator *(double factor, Current value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(Current left, Current right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Current number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Current angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Current other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Current left, Current right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Current left, Current right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Current left, Current right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Current left, Current right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Current left, Current right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Current left, Current right) => left._Value >= right._Value;

    #endregion
}
