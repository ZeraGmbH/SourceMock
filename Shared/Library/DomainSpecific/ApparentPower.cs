using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// apparent power (in VA) as domain specific number.
/// </summary>
public readonly struct ApparentPower(double value) : IInternalDomainSpecificNumber<ApparentPower>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static ApparentPower Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VA";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ApparentPower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some apparent power.</param>
    public static explicit operator double(ApparentPower power) => power._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="phaseAngle"></param>
    /// <returns></returns>
    public ActivePower GetActivePower(Angle phaseAngle) => new(_Value * phaseAngle.Cos());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="phaseAngle"></param>
    /// <returns></returns>
    public ReactivePower GetReactivePower(Angle phaseAngle) => new(_Value * phaseAngle.Sin());

    #region Arithmetics

    /// <inheritdoc/>
    public static ApparentPower operator +(ApparentPower left, ApparentPower right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ApparentPower operator -(ApparentPower left, ApparentPower right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ApparentPower operator -(ApparentPower value) => new(-value._Value);

    /// <inheritdoc/>
    public static ApparentPower operator %(ApparentPower left, ApparentPower right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ApparentPower operator /(ApparentPower value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ApparentPower operator *(ApparentPower value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ApparentPower operator *(double factor, ApparentPower value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ApparentPower left, ApparentPower right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ApparentPower number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ApparentPower angle && _Value == angle._Value;

    /// <inheritdoc/>
    public int CompareTo(ApparentPower other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public static bool operator ==(ApparentPower left, ApparentPower right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ApparentPower left, ApparentPower right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ApparentPower left, ApparentPower right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ApparentPower left, ApparentPower right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ApparentPower left, ApparentPower right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ApparentPower left, ApparentPower right) => left._Value >= right._Value;

    #endregion
}
