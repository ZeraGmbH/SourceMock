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

    /// <inheritdoc/>
    public static bool operator !(ReactivePower power) => power._Value == 0;

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
}
