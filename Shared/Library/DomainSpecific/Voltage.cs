namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Voltage (V) as domain specific number.
/// </summary>
public class Voltage(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "V";

    /// <summary>
    /// No Voltage at all.
    /// </summary>
    public static readonly Voltage Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    public static explicit operator double(Voltage Voltage) => Voltage.Value;

    /// <summary>
    /// Add to Voltage.
    /// </summary>
    /// <param name="left">One Voltage.</param>
    /// <param name="right">Another Voltage.</param>
    /// <returns>New Voltage instance representing the sum of the parameters.</returns>
    public static Voltage operator +(Voltage left, Voltage right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale Voltage by a factor.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    /// <param name="factor">Factor to apply to the Voltage.</param>
    /// <returns>New Voltage with scaled value.</returns>
    public static Voltage operator *(Voltage Voltage, double factor) => new(Voltage.Value * factor);

    /// <summary>
    /// Scale Voltage by a factor.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    /// <param name="factor">Factor to apply to the Voltage.</param>
    /// <returns>New Voltage with scaled value.</returns>
    public static Voltage operator *(double factor, Voltage Voltage) => new(factor * Voltage.Value);
}
