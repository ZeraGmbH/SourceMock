using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Power factor as domain specific number.
/// </summary>
/// <param name="value">Factor to use.</param>
public readonly struct PowerFactor(double value) : IInternalDomainSpecificNumber<PowerFactor>
{
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
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static PowerFactor Create(double value) => new(value);

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
    public static explicit operator Angle(PowerFactor powerFactor) => Angle.Acos(powerFactor._Value);

    #region Arithmetics

    /// <inheritdoc/>
    public static PowerFactor operator +(PowerFactor left, PowerFactor right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static PowerFactor operator -(PowerFactor left, PowerFactor right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static PowerFactor operator -(PowerFactor value) => new(-value._Value);

    /// <inheritdoc/>
    public static PowerFactor operator %(PowerFactor left, PowerFactor right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static PowerFactor operator /(PowerFactor value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static PowerFactor operator *(PowerFactor value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static PowerFactor operator *(double factor, PowerFactor value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(PowerFactor left, PowerFactor right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(PowerFactor number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is PowerFactor angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(PowerFactor other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(PowerFactor left, PowerFactor right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(PowerFactor left, PowerFactor right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(PowerFactor left, PowerFactor right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(PowerFactor left, PowerFactor right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(PowerFactor left, PowerFactor right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(PowerFactor left, PowerFactor right) => left._Value >= right._Value;

    #endregion
}
