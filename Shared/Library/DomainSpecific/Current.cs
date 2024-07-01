using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Current (A) as domain specific number.
/// </summary>
public readonly struct Current(double value) : IDomainSpecificNumber
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "A";

    /// <summary>
    /// No Current at all.
    /// </summary>
    public static readonly Current Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    public static explicit operator double(Current Current) => Current._Value;

    /// <summary>
    /// Add to Current.
    /// </summary>
    /// <param name="left">One Current.</param>
    /// <param name="right">Another Current.</param>
    /// <returns>New Current instance representing the sum of the parameters.</returns>
    public static Current operator +(Current left, Current right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="Current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(Current Current, double factor) => new(Current._Value * factor);

    /// <summary>
    /// Scale Current by a factor.
    /// </summary>
    /// <param name="current">Some Current.</param>
    /// <param name="factor">Factor to apply to the Current.</param>
    /// <returns>New Current with scaled value.</returns>
    public static Current operator *(double factor, Current current) => new(factor * current._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="voltage"></param>
    /// <returns></returns>
    public static ActivePower operator *(Current current, Voltage voltage) => new(current._Value * (double)voltage);
}
