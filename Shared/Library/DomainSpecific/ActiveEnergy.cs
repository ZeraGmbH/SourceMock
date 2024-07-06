using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Active energy (in Wh) as domain specific number.
/// </summary>
public readonly struct ActiveEnergy(double value) : IInternalDomainSpecificNumber<ActiveEnergy>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static ActiveEnergy Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "Wh";

    /// <summary>
    /// No energy at all.
    /// </summary>
    public static readonly ActiveEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some active energy.</param>
    public static explicit operator double(ActiveEnergy energy) => energy._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="energy"></param>
    /// <param name="meterConstant"></param>
    /// <returns></returns>
    public static Impulses operator *(ActiveEnergy energy, MeterConstant meterConstant) => meterConstant * energy;

    /// <summary>
    /// Get time from power and energy
    /// </summary>
    /// <param name="power">Some energy.</param>
    /// <param name="energy">Some power.</param>
    /// <returns>time in seconds.</returns>
    public static Time? operator /(ActiveEnergy energy, ActivePower power) => (double)power == 0 ? null : new(energy._Value / (double)power * 3600);

    #region Arithmetics

    /// <inheritdoc/>
    public static ActiveEnergy operator +(ActiveEnergy left, ActiveEnergy right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ActiveEnergy operator -(ActiveEnergy left, ActiveEnergy right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ActiveEnergy operator -(ActiveEnergy value) => new(-value._Value);

    /// <inheritdoc/>
    public static ActiveEnergy operator %(ActiveEnergy left, ActiveEnergy right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ActiveEnergy operator *(ActiveEnergy value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ActiveEnergy operator /(ActiveEnergy value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ActiveEnergy operator *(double factor, ActiveEnergy value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ActiveEnergy left, ActiveEnergy right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ActiveEnergy number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ActiveEnergy angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ActiveEnergy other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(ActiveEnergy left, ActiveEnergy right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ActiveEnergy left, ActiveEnergy right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ActiveEnergy left, ActiveEnergy right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ActiveEnergy left, ActiveEnergy right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ActiveEnergy left, ActiveEnergy right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ActiveEnergy left, ActiveEnergy right) => left._Value >= right._Value;

    #endregion
}
