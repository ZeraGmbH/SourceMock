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
    public static Frequency operator +(Frequency left, Frequency right) => new(left._Value + right._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Frequency operator -(Frequency left, Frequency right) => new(left._Value - right._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="factor"></param>
    /// <returns></returns>
    public static Frequency operator *(Frequency left, double factor) => new(left._Value * factor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    public static Frequency operator *(double factor, Frequency left) => left * factor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator <=(Frequency left, Frequency right) => left._Value <= right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator >=(Frequency left, Frequency right) => left._Value <= right._Value;

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
}
