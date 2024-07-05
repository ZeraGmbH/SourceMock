using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Current (A) as domain specific number.
/// </summary>
public readonly struct Current(double value) : IInternalDomainSpecificNumber<Current>, IComparable<Current>
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
    /// Set if the current is zero.
    /// </summary>
    public static bool operator !(Current current) => current._Value == 0;

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "A";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    public static explicit operator double(Current Current) => Current._Value;

    /// <summary>
    /// Add to Current.
    /// </summary>
    /// <param name="left">One Current.</param>
    /// <param name="right">Another Current.</param>
    /// <returns>New Current instance representing the sum of the parameters.</returns>
    public static Current operator +(Current left, Current right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(Current Current, double factor) => new(Current._Value * factor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Current operator %(Current Current, Current value) => new(Current._Value % value._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Current operator -(Current Current, Current value) => new(Current._Value - value._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator <(Current Current, Current value) => Current._Value < value._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator >(Current Current, Current value) => Current._Value > value._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator <=(Current Current, Current value) => Current._Value <= value._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool operator >=(Current Current, Current value) => Current._Value >= value._Value;

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(double factor, Current current) => new(factor * current._Value);

    /// <summary>
    /// Negation operator
    /// </summary>
    /// <param name="value">One Voltage.</param>
    /// <returns>New Voltage instance representing the substraction of the parameters.</returns>
    public static Current operator -(Current value) => new(-value._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="voltage"></param>
    /// <returns></returns>
    public static ApparentPower operator *(Current current, Voltage voltage) => new(current._Value * (double)voltage);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double operator /(Current left, Current right) => left._Value / right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Current Abs() => new(Math.Abs(_Value));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Current Max(Current left, Current right) => left._Value >= right._Value ? left : right;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Current Min(Current left, Current right) => left._Value <= right._Value ? left : right;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Current other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is Current angle && _Value == angle._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>+
    public override int GetHashCode() => _Value.GetHashCode();

}
