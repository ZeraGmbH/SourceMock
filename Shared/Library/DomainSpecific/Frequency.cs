using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Frequency as domain specific number.
/// </summary>
/// <param name="value">Factor to use.</param>
public readonly struct Frequency(double value) : IInternalDomainSpecificNumber<Frequency>
{

    /// <summary>
    /// Create zwero frequency..
    /// </summary>
    public Frequency() : this(0) { }

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static Frequency Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "Hz";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="frequency">Some frequency.</param>
    public static explicit operator double(Frequency frequency) => frequency._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Frequency Max(Frequency left, Frequency right) => left._Value >= right._Value ? left : right;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Frequency Min(Frequency left, Frequency right) => left._Value <= right._Value ? left : right;

    #region Arithmetics

    /// <inheritdoc/>
    public static Frequency operator +(Frequency left, Frequency right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Frequency operator -(Frequency left, Frequency right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Frequency operator -(Frequency value) => new(-value._Value);

    /// <inheritdoc/>
    public static Frequency operator %(Frequency left, Frequency right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Frequency operator *(Frequency value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static Frequency operator /(Frequency value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Frequency operator *(double factor, Frequency value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(Frequency left, Frequency right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Frequency number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Frequency angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Frequency other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Frequency left, Frequency right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Frequency left, Frequency right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Frequency left, Frequency right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Frequency left, Frequency right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Frequency left, Frequency right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Frequency left, Frequency right) => left._Value >= right._Value;

    #endregion
}
