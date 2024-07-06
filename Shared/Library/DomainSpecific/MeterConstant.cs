using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// MeterConstant (Impulses/kWh) as domain specific number.
/// </summary>
public readonly struct MeterConstant(double value) : IInternalDomainSpecificNumber<MeterConstant>
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
    /// <param name="meterConstant">Imp/kWh</param>
    /// <param name="energy">W</param>
    /// <returns>Imp -> Constant</returns>
    public static Impulses operator *(MeterConstant meterConstant, ActiveEnergy energy) => new((double)meterConstant * (double)energy / 1000);


    #region Comparable

    /// <summary>
    /// See if the number is exactly zero.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>Set if number is zero.</returns>
    public static bool operator !(MeterConstant number) => number._Value == 0;

    /// <summary>
    /// Compare with any other object.
    /// </summary>
    /// <param name="obj">Some other object.</param>
    /// <returns>Set if this number is identical to the other object.</returns>
    public override bool Equals(object? obj) => obj is MeterConstant angle && _Value == angle._Value;

    /// <summary>
    /// Get a hashcode.
    /// </summary>
    /// <returns>Hashcode for this number.</returns>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <summary>
    /// Compare to another number.
    /// </summary>
    /// <param name="other">The other number.</param>
    /// <returns>Comparision result of the number.</returns>
    public int CompareTo(MeterConstant other) => _Value.CompareTo(other._Value);

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are exactly identical.</returns>
    public static bool operator ==(MeterConstant left, MeterConstant right) => left._Value == right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are not exactly identical.</returns>
    public static bool operator !=(MeterConstant left, MeterConstant right) => left._Value != right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is less than the second number.</returns>
    public static bool operator <(MeterConstant left, MeterConstant right) => left._Value < right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not greater than the second number.</returns>
    public static bool operator <=(MeterConstant left, MeterConstant right) => left._Value <= right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is greater than the second number.</returns>
    public static bool operator >(MeterConstant left, MeterConstant right) => left._Value > right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not less than the second number.</returns>
    public static bool operator >=(MeterConstant left, MeterConstant right) => left._Value >= right._Value;

    #endregion
}
