using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Impulses (a constant number) as domain specific number.
/// </summary>
public readonly struct Impulses(long value) : IDomainSpecificNumber
{
    /// <summary>
    /// The real value is always represented as a long.
    /// </summary>
    private readonly long _Value = value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "";

    /// <summary>
    /// No Impulses at all.
    /// </summary>
    public static readonly Impulses Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Impulses">Some active Impulses.</param>
    public static explicit operator long(Impulses Impulses) => Impulses._Value;

    /// <summary>
    /// Add to Impulses.
    /// </summary>
    /// <param name="left">One Impulses.</param>
    /// <param name="right">Another Impulses.</param>
    /// <returns>New Impulses instance representing the sum of the parameters.</returns>
    public static Impulses operator +(Impulses left, Impulses right) => new(left._Value + right._Value);

    /// <summary>
    /// Add to Impulses.
    /// </summary>
    /// <param name="impulses">One Impulses.</param>
    /// <param name="value">Another Impulses.</param>
    /// <returns>New Impulses instance representing the sum of the parameters.</returns>
    public static Impulses operator +(Impulses impulses, int value) => new(impulses._Value + value);

    /// <summary>
    /// Scale Impulses by a factor.
    /// </summary>
    /// <param name="impulses">Some Impulses.</param>
    /// <param name="factor">Factor to apply to the Impulses.</param>
    /// <returns>New Impulses with scaled value.</returns>
    public static Impulses operator *(Impulses impulses, long factor) => new(impulses._Value * factor);

    /// <summary>
    /// Devide impulses with meterConstant to get ActiveEnergy
    /// </summary>
    /// <param name="impulses">Some Impulses.</param>
    /// <param name="meterConstant">MeterConstant.</param>
    /// <returns>New ActiveEnergy.</returns>
    public static ActiveEnergy operator /(Impulses impulses, MeterConstant meterConstant) => new(impulses._Value / (double)meterConstant);
}
