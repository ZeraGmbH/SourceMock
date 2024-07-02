using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Power factor as domain specific number.
/// </summary>
public readonly struct PowerFactor : IInternalDomainSpecificNumber
{
    /// <summary>
    /// Create a power factor.
    /// </summary>
    /// <param name="value">Factor to use.</param>
    /// <exception cref="ArgumentOutOfRangeException">Value must be between 0 and 1.</exception>
    public PowerFactor(double value)
    {
        if (value < 0d || value > 1d)
            throw new ArgumentOutOfRangeException(nameof(value), "Power Factor must be between 0 and 1 - both inclusiv.");

        _Value = value;
    }

    /// <summary>
    /// Create power factor 1.
    /// </summary>
    public PowerFactor() : this(1) { }

    /// <summary>
    /// Calculate power factor from angle.
    /// </summary>
    /// <param name="angle"></param>
    public PowerFactor(Angle angle) : this(angle.Cos()) { }

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="powerFactor">Some power factor.</param>
    public static explicit operator double(PowerFactor powerFactor) => powerFactor._Value;

    /// <summary>
    /// Convert a power factor to the corresponding angle between voltage and current.
    /// </summary>
    /// <param name="powerFactor">The power factor.</param>
    public static explicit operator Angle(PowerFactor powerFactor) => new(Math.Acos(powerFactor._Value) * 180d / Math.PI);
}
