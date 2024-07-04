using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Frequency as domain specific number.
/// </summary>
/// <param name="value">Factor to use.</param>
public readonly struct Frequency(double value) : IInternalDomainSpecificNumber
{

    /// <summary>
    /// Create zwero frequency..
    /// </summary>
    public Frequency() : this(0) { }

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "Hz";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="frequency">Some frequency.</param>
    public static explicit operator double(Frequency frequency) => frequency._Value;
}
