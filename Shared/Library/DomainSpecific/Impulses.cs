namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Impulses (a constant number) as domain specific number.
/// </summary>
public class Impulses(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "";

    /// <summary>
    /// No Impulses at all.
    /// </summary>
    public static readonly Impulses Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Impulses">Some active Impulses.</param>
    public static explicit operator double(Impulses Impulses) => Impulses.Value;

    /// <summary>
    /// Add to Impulses.
    /// </summary>
    /// <param name="left">One Impulses.</param>
    /// <param name="right">Another Impulses.</param>
    /// <returns>New Impulses instance representing the sum of the parameters.</returns>
    public static Impulses operator +(Impulses left, Impulses right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale Impulses by a factor.
    /// </summary>
    /// <param name="Impulses">Some Impulses.</param>
    /// <param name="factor">Factor to apply to the Impulses.</param>
    /// <returns>New Impulses with scaled value.</returns>
    public static Impulses operator *(Impulses Impulses, double factor) => new(Impulses.Value * factor);

    /// <summary>
    /// Scale Impulses by a factor.
    /// </summary>
    /// <param name="Impulses">Some Impulses.</param>
    /// <param name="factor">Factor to apply to the Impulses.</param>
    /// <returns>New Impulses with scaled value.</returns>
    public static Impulses operator *(double factor, Impulses Impulses) => new(factor * Impulses.Value);
}
