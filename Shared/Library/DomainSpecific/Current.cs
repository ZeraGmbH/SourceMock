namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Current (A) as domain specific number.
/// </summary>
public class Current(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "A";

    /// <summary>
    /// No Current at all.
    /// </summary>
    public static readonly Current Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    public static explicit operator double(Current Current) => Current.Value;

    /// <summary>
    /// Add to Current.
    /// </summary>
    /// <param name="left">One Current.</param>
    /// <param name="right">Another Current.</param>
    /// <returns>New Current instance representing the sum of the parameters.</returns>
    public static Current operator +(Current left, Current right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(Current Current, double factor) => new(Current.Value * factor);

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(double factor, Current Current) => new(factor * Current.Value);
}
