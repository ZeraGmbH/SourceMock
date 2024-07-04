using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active power (in W) as domain specific number.
/// </summary>
public readonly struct ActivePower(double value) : IInternalDomainSpecificNumber<ActivePower>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "W";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ActivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ActivePower power) => power._Value;

    /// <summary>
    /// Add to active power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New active power instance representing the sum of the parameters.</returns>
    public static ActivePower operator +(ActivePower left, ActivePower right) => new(left._Value + right._Value);

    /// <summary>
    /// Subtract from active power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New active power instance representing the sum of the parameters.</returns>
    public static ActivePower operator -(ActivePower left, ActivePower right) => new(left._Value - right._Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ActivePower operator *(ActivePower power, double factor) => new(power._Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ActivePower operator *(double factor, ActivePower power) => new(factor * power._Value);

    /// <summary>
    /// Divide by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ActivePower operator /(ActivePower power, double factor) => new(power._Value / factor);

    /// <summary>
    /// Calulate the power factor.
    /// </summary>
    /// <param name="power">Active power.</param>
    /// <param name="apparent">Apparent power.</param>
    /// <returns>Ratio of active to apparent power or null if apparent power is zero.</returns>
    public static PowerFactor? operator /(ActivePower power, ApparentPower apparent) => (double)apparent == 0 ? null : new(power._Value / (double)apparent);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="power"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static ActiveEnergy operator *(ActivePower power, Time time) => new(power._Value * (double)time / 3600d);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ActivePower Abs() => new(Math.Abs(_Value));

}
