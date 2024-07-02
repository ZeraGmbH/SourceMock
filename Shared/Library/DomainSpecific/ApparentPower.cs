using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// apparent power (in VA) as domain specific number.
/// </summary>
public readonly struct ApparentPower(double value) : IInternalDomainSpecificNumber
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VA";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ApparentPower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some apparent power.</param>
    public static explicit operator double(ApparentPower power) => power._Value;

    /// <summary>
    /// Add to apparent power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New apparent power instance representing the sum of the parameters.</returns>
    public static ApparentPower operator +(ApparentPower left, ApparentPower right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ApparentPower operator *(ApparentPower power, double factor) => new(power._Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ApparentPower operator *(double factor, ApparentPower power) => new(factor * power._Value);
}
