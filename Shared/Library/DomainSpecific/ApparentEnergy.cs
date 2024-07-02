using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// apparent energy (in VAh) as domain specific number.
/// </summary>
public readonly struct ApparentEnergy(double value) : IInternalDomainSpecificNumber
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VAh";

    /// <summary>
    /// No energy at all.
    /// </summary>
    public static readonly ApparentEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some apparent energy.</param>
    public static explicit operator double(ApparentEnergy energy) => energy._Value;

    /// <summary>
    /// Add to apparent energies.
    /// </summary>
    /// <param name="left">One energy.</param>
    /// <param name="right">Another energy.</param>
    /// <returns>New apparent energy instance representing the sum of the parameters.</returns>
    public static ApparentEnergy operator +(ApparentEnergy left, ApparentEnergy right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ApparentEnergy operator *(ApparentEnergy energy, double factor) => new(energy._Value * factor);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ApparentEnergy operator *(double factor, ApparentEnergy energy) => new(factor * energy._Value);
}
