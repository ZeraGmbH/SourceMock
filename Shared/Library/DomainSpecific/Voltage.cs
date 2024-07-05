using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Voltage (V) as domain specific number.
/// </summary>
public readonly struct Voltage(double value) : IInternalDomainSpecificNumber<Voltage>, IComparable<Voltage>
{
    /// <summary>
    /// Create voltage 0.
    /// </summary>
    public Voltage() : this(0) { }

    /// <summary>
    /// Helper holding 0 volts.
    /// </summary>
    public static readonly Voltage Zero = new();

    /// <inheritdoc/>
    public static bool operator !(Voltage voltage) => voltage._Value == 0;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "V";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    public static explicit operator double(Voltage Voltage) => Voltage._Value;

    /// <summary>
    /// Add to Voltage.
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns>New Voltage instance representing the sum of the parameters.</returns>
    public static Voltage operator +(Voltage left, Voltage right) => new(left._Value + right._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns>New Voltage instance representing the substraction of the parameters.</returns>
    public static Voltage operator -(Voltage left, Voltage right) => new(left._Value - right._Value);

    /// <summary>
    /// Negation operator
    /// </summary>
    /// <param name="value">One Voltage.</param>
    /// <returns>New Voltage instance representing the substraction of the parameters.</returns>
    public static Voltage operator -(Voltage value) => new(-value._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns>New Voltage instance representing the modulo operation of the parameters.</returns>
    public static Voltage operator %(Voltage left, Voltage right) => new(left._Value % right._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns></returns>
    public static bool operator <(Voltage left, Voltage right) => left._Value < right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns></returns>
    public static bool operator >(Voltage left, Voltage right) => left._Value > right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns></returns>
    public static bool operator <=(Voltage left, Voltage right) => left._Value <= right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns></returns>
    public static bool operator >=(Voltage left, Voltage right) => left._Value >= right._Value;

    /// <summary>
    /// Scale Voltage by a factor.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    /// <param name="factor">Factor to apply to the Voltage.</param>
    /// <returns>New Voltage with scaled value.</returns>
    public static Voltage operator *(Voltage Voltage, double factor) => new(Voltage._Value * factor);

    /// <summary>
    /// Scale Voltage by a factor.
    /// </summary>
    /// <param name="voltage">Some Voltage.</param>
    /// <param name="factor">Factor to apply to the Voltage.</param>
    /// <returns>New Voltage with scaled value.</returns>
    public static Voltage operator *(double factor, Voltage voltage) => new(factor * voltage._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="voltage"></param>
    /// <returns></returns>
    public static ApparentPower operator *(Voltage voltage, Current current) => new(voltage._Value * (double)current);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double operator /(Voltage left, Voltage right) => left._Value / right._Value;

    /// <summary>
    /// 
    /// </summary>
    public Voltage Abs() => new(Math.Abs(_Value));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Voltage Max(Voltage left, Voltage right) => left._Value >= right._Value ? left : right;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Voltage Min(Voltage left, Voltage right) => left._Value <= right._Value ? left : right;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Voltage other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is Voltage angle && _Value == angle._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>+
    public override int GetHashCode() => _Value.GetHashCode();

}
