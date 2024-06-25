namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active energy (in Wh) as domain specific number.
/// </summary>
public class ActiveEnergie(double value) : DomainSpecificNumber(value)
{
    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some active energy.</param>
    public static explicit operator double(ActiveEnergie energy) => energy.Value;

    /// <summary>
    /// Add to active energies.
    /// </summary>
    /// <param name="left">One energy.</param>
    /// <param name="right">Another energy.</param>
    /// <returns>New active energy instance representing the sum of the parameters.</returns>
    public static ActiveEnergie operator +(ActiveEnergie left, ActiveEnergie right) => new(left.Value + right.Value);
}
