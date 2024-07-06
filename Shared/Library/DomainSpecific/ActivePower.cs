using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active power (in W) as domain specific number.
/// </summary>
public readonly struct ActivePower(double value) : IInternalDomainSpecificNumber<ActivePower>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "W";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ActivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ActivePower power) => power._Value;

    /// <summary>
    /// Calulate the power factor.
    /// </summary>
    /// <param name="power">Active power.</param>
    /// <param name="apparent">Apparent power.</param>
    /// <returns>Ratio of active to apparent power or null if apparent power is zero.</returns>
    public static PowerFactor? operator /(ActivePower power, ApparentPower apparent) => (double)apparent == 0 ? null : new(power._Value / (double)apparent);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="power"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static ActiveEnergy operator *(ActivePower power, Time time) => new(power._Value * (double)time / 3600d);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ActivePower Abs() => new(Math.Abs(_Value));

    #region Arithmetics

    /// <inheritdoc/>
    public static ActivePower operator +(ActivePower left, ActivePower right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ActivePower operator -(ActivePower left, ActivePower right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ActivePower operator -(ActivePower value) => new(-value._Value);

    /// <inheritdoc/>
    public static ActivePower operator %(ActivePower left, ActivePower right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ActivePower operator *(ActivePower value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ActivePower operator /(ActivePower value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ActivePower operator *(double factor, ActivePower value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ActivePower left, ActivePower right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ActivePower number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ActivePower angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ActivePower other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(ActivePower left, ActivePower right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ActivePower left, ActivePower right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ActivePower left, ActivePower right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ActivePower left, ActivePower right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ActivePower left, ActivePower right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ActivePower left, ActivePower right) => left._Value >= right._Value;

    #endregion
}
