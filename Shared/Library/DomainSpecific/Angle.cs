using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Angle (°) as domain specific number.
/// </summary>
public readonly struct Angle(double value) : IInternalDomainSpecificNumber<Angle>
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly double DegToRad = Math.PI / 180d;

    /// <summary>
    /// Create Angle 0.
    /// </summary>
    public Angle() : this(0) { }

    /// <summary>
    /// No angle at all.
    /// </summary>
    public static readonly Angle Zero = new();

    /// <summary>
    /// Full circle.
    /// </summary>
    public static readonly Angle Full = new(360);

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static Angle Create(double value) => new(value);

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "°";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Angle">Some Angle.</param>
    public static explicit operator double(Angle Angle) => Angle._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double Sin() => Math.Sin(ToRad());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double Cos() => Math.Cos(ToRad());

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Angle Abs() => new(Math.Abs(_Value));

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public double ToRad() => _Value * DegToRad;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Angle FromRad(double angle) => new(angle / DegToRad);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Angle Acos(double num) => FromRad(Math.Acos(num));

    /// <summary>
    /// Create a new angle which is normalized to 0° (inclusive)
    /// and 360° (exclusive).
    /// </summary>
    public Angle Normalize() => new((_Value % 360d + 360d) % 360d);

    #region Arithmetics

    /// <inheritdoc/>
    public static Angle operator +(Angle left, Angle right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Angle operator -(Angle left, Angle right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Angle operator -(Angle value) => new(-value._Value);

    /// <inheritdoc/>
    public static Angle operator %(Angle left, Angle right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Angle operator *(Angle value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static Angle operator /(Angle value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Angle operator *(double factor, Angle value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(Angle left, Angle right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Angle number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Angle angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Angle other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Angle left, Angle right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Angle left, Angle right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Angle left, Angle right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Angle left, Angle right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Angle left, Angle right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Angle left, Angle right) => left._Value >= right._Value;

    #endregion
}
