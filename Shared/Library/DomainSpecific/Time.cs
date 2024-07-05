using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Time (in s) as domain specific number.
/// </summary>
public readonly struct Time(double value) : IInternalDomainSpecificNumber
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool operator >(Time value1, Time value2) => value1._Value > value2._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool operator <(Time value1, Time value2) => value1._Value < value2._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool operator >=(Time value1, Time value2) => value1._Value >= value2._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
    public static bool operator <=(Time value1, Time value2) => value1._Value <= value2._Value;
}
