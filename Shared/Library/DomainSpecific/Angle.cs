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
    /// Add to Angle.
    /// </summary>
    /// <param name="left">One Angle.</param>
    /// <param name="right">Another Angle.</param>
    /// <returns>New Angle instance representing the sum of the parameters.</returns>
    public static Angle operator +(Angle left, Angle right) => new(left._Value + right._Value);

    /// <summary>
    /// Substract from Angle.
    /// </summary>
    /// <param name="left">One Angle.</param>
    /// <param name="right">Another Angle.</param>
    /// <returns>New Angle instance representing the substraction of the parameters.</returns>
    public static Angle operator -(Angle left, Angle right) => new(left._Value - right._Value);

    /// <summary>
    /// Modulo operation on angle.
    /// </summary>
    /// <param name="left">One Angle.</param>
    /// <param name="number">Some number.</param>
    /// <returns>New Angle instance.</returns>
    public static Angle operator %(Angle left, double number) => new(left._Value % number);


    /// <summary>
    /// Division on angle.
    /// </summary>
    /// <param name="left">One Angle.</param>
    /// <param name="number">Some number.</param>
    /// <returns>New Angle instance.</returns>
    public static Angle operator /(Angle left, double number) => new(left._Value / number);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double operator /(Angle left, Angle right) => left._Value / right._Value;

    /// <summary>
    /// Scale Angle by a factor.
    /// </summary>
    /// <param name="angle">Some Angle.</param>
    /// <param name="factor">Factor to apply to the Angle.</param>
    /// <returns>New Angle with scaled value.</returns>
    public static Angle operator *(Angle angle, double factor) => new(angle._Value * factor);

    /// <summary>
    /// Scale Angle by a factor.
    /// </summary>
    /// <param name="angle">Some Angle.</param>
    /// <param name="factor">Factor to apply to the Angle.</param>
    /// <returns>New Angle with scaled value.</returns>
    public static Angle operator *(double factor, Angle angle) => angle * factor;

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

    #region Comparable

    /// <summary>
    /// See if the number is exactly zero.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>Set if number is zero.</returns>
    public static bool operator !(Angle number) => number._Value == 0;

    /// <summary>
    /// Compare with any other object.
    /// </summary>
    /// <param name="obj">Some other object.</param>
    /// <returns>Set if this number is identical to the other object.</returns>
    public override bool Equals(object? obj) => obj is Angle angle && _Value == angle._Value;

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
    public int CompareTo(Angle other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are exactly identical.</returns>
    public static bool operator ==(Angle left, Angle right) => left._Value == right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are not exactly identical.</returns>
    public static bool operator !=(Angle left, Angle right) => left._Value != right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is less than the second number.</returns>
    public static bool operator <(Angle left, Angle right) => left._Value < right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not greater than the second number.</returns>
    public static bool operator <=(Angle left, Angle right) => left._Value <= right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is greater than the second number.</returns>
    public static bool operator >(Angle left, Angle right) => left._Value > right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not less than the second number.</returns>
    public static bool operator >=(Angle left, Angle right) => left._Value >= right._Value;

    #endregion
}
