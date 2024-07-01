namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active power (in W) as domain specific number.
/// </summary>
public class ActivePower(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "W";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ActivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ActivePower power) => power.Value;

    /// <summary>
    /// Add to active power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New active power instance representing the sum of the parameters.</returns>
    public static ActivePower operator +(ActivePower left, ActivePower right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ActivePower operator *(ActivePower power, double factor) => new(power.Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ActivePower operator *(double factor, ActivePower power) => new(factor * power.Value);
}
