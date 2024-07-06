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
    /// 
    /// </summary>
    /// <param name="meterConstant">Imp/kWh</param>
    /// <param name="energy">W</param>
    /// <returns>Imp -> Constant</returns>
    public static Impulses operator *(MeterConstant meterConstant, ActiveEnergy energy) => new(meterConstant._Value * (double)energy / 1000d);

    #region Arithmetics

    /// <inheritdoc/>
    public static MeterConstant operator +(MeterConstant left, MeterConstant right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static MeterConstant operator -(MeterConstant left, MeterConstant right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static MeterConstant operator -(MeterConstant value) => new(-value._Value);

    /// <inheritdoc/>
    public static MeterConstant operator %(MeterConstant left, MeterConstant right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static MeterConstant operator /(MeterConstant value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static MeterConstant operator *(MeterConstant value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static MeterConstant operator *(double factor, MeterConstant value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(MeterConstant left, MeterConstant right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(MeterConstant number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is MeterConstant angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(MeterConstant other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(MeterConstant left, MeterConstant right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(MeterConstant left, MeterConstant right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(MeterConstant left, MeterConstant right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(MeterConstant left, MeterConstant right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(MeterConstant left, MeterConstant right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(MeterConstant left, MeterConstant right) => left._Value >= right._Value;

    #endregion
}
