namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active energy (in Wh) as domain specific number.
/// </summary>
public class ActiveEnergy(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "Wh";

    /// <summary>
    /// No enegery at all.
    /// </summary>
    public static readonly ActiveEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some active energy.</param>
    public static explicit operator double(ActiveEnergy energy) => energy.Value;

    /// <summary>
    /// Add to active energies.
    /// </summary>
    /// <param name="left">One energy.</param>
    /// <param name="right">Another energy.</param>
    /// <returns>New active energy instance representing the sum of the parameters.</returns>
    public static ActiveEnergy operator +(ActiveEnergy left, ActiveEnergy right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ActiveEnergy operator *(ActiveEnergy energy, double factor) => new(energy.Value * factor);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ActiveEnergy operator *(double factor, ActiveEnergy energy) => new(factor * energy.Value);
}
