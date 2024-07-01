namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Rective power (in VAr) as domain specific number.
/// </summary>
public class ReactivePower(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "VAr";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ReactivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ReactivePower power) => power.Value;

    /// <summary>
    /// Add to reactive power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New reactive power instance representing the sum of the parameters.</returns>
    public static ReactivePower operator +(ReactivePower left, ReactivePower right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ReactivePower operator *(ReactivePower power, double factor) => new(power.Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ReactivePower operator *(double factor, ReactivePower power) => new(factor * power.Value);
}
