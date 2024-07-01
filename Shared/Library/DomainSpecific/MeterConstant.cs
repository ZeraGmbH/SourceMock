namespace SharedLibrary.DomainSpecific;

/// <summary>
/// MeterConstant (Impulses/kWh) as domain specific number.
/// </summary>
public class MeterConstant(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "Imp/kWh";

    /// <summary>
    /// No MeterConstant at all.
    /// </summary>
    public static readonly MeterConstant Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    public static explicit operator double(MeterConstant MeterConstant) => MeterConstant.Value;

    /// <summary>
    /// Add to MeterConstant.
    /// </summary>
    /// <param name="left">One MeterConstant.</param>
    /// <param name="right">Another MeterConstant.</param>
    /// <returns>New MeterConstant instance representing the sum of the parameters.</returns>
    public static MeterConstant operator +(MeterConstant left, MeterConstant right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale MeterConstant by a factor.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    /// <param name="factor">Factor to apply to the MeterConstant.</param>
    /// <returns>New MeterConstant with scaled value.</returns>
    public static MeterConstant operator *(MeterConstant MeterConstant, double factor) => new(MeterConstant.Value * factor);

    /// <summary>
    /// Scale MeterConstant by a factor.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    /// <param name="factor">Factor to apply to the MeterConstant.</param>
    /// <returns>New MeterConstant with scaled value.</returns>
    public static MeterConstant operator *(double factor, MeterConstant MeterConstant) => new(factor * MeterConstant.Value);
}
