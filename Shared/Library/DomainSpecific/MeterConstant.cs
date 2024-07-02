using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// MeterConstant (Impulses/kWh) as domain specific number.
/// </summary>
public readonly struct MeterConstant(double value) : IInternalDomainSpecificNumber
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "Imp/kWh";

    /// <summary>
    /// No MeterConstant at all.
    /// </summary>
    public static readonly MeterConstant Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    public static explicit operator double(MeterConstant MeterConstant) => MeterConstant._Value;

    /// <summary>
    /// Add to MeterConstant.
    /// </summary>
    /// <param name="left">One MeterConstant.</param>
    /// <param name="right">Another MeterConstant.</param>
    /// <returns>New MeterConstant instance representing the sum of the parameters.</returns>
    public static MeterConstant operator +(MeterConstant left, MeterConstant right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale MeterConstant by a factor.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    /// <param name="factor">Factor to apply to the MeterConstant.</param>
    /// <returns>New MeterConstant with scaled value.</returns>
    public static MeterConstant operator *(MeterConstant MeterConstant, double factor) => new(MeterConstant._Value * factor);

    /// <summary>
    /// Scale MeterConstant by a factor.
    /// </summary>
    /// <param name="MeterConstant">Some MeterConstant.</param>
    /// <param name="factor">Factor to apply to the MeterConstant.</param>
    /// <returns>New MeterConstant with scaled value.</returns>
    public static MeterConstant operator *(double factor, MeterConstant MeterConstant) => new(factor * MeterConstant._Value);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="meterConstant"></param>
    /// <param name="value"></param>
    /// <returns>true if value is smaller, false otherwise</returns>
    public static bool operator <=(MeterConstant meterConstant, int value) => meterConstant._Value <= value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="meterConstant"></param>
    /// <param name="value"></param>
    /// <returns>true if value is bigger, false otherwise</returns>
    public static bool operator >=(MeterConstant meterConstant, int value) => meterConstant._Value >= value;
}
