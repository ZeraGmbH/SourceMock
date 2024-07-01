namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Rective energy (in VArh) as domain specific number.
/// </summary>
public class ReactiveEnergy(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "VArh";

    /// <summary>
    /// No energy at all.
    /// </summary>
    public static readonly ReactiveEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some active energy.</param>
    public static explicit operator double(ReactiveEnergy energy) => energy.Value;

    /// <summary>
    /// Add to reactive energies.
    /// </summary>
    /// <param name="left">One energy.</param>
    /// <param name="right">Another energy.</param>
    /// <returns>New reactive energy instance representing the sum of the parameters.</returns>
    public static ReactiveEnergy operator +(ReactiveEnergy left, ReactiveEnergy right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ReactiveEnergy operator *(ReactiveEnergy energy, double factor) => new(energy.Value * factor);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ReactiveEnergy operator *(double factor, ReactiveEnergy energy) => new(factor * energy.Value);
}
