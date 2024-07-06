using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// apparent energy (in VAh) as domain specific number.
/// </summary>
public readonly struct ApparentEnergy(double value) : IInternalDomainSpecificNumber<ApparentEnergy>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VAh";

    /// <summary>
    /// No energy at all.
    /// </summary>
    public static readonly ApparentEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some apparent energy.</param>
    public static explicit operator double(ApparentEnergy energy) => energy._Value;

    #region Arithmetics

    /// <inheritdoc/>
    public static ApparentEnergy operator +(ApparentEnergy left, ApparentEnergy right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ApparentEnergy operator -(ApparentEnergy left, ApparentEnergy right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ApparentEnergy operator -(ApparentEnergy value) => new(-value._Value);

    /// <inheritdoc/>
    public static ApparentEnergy operator %(ApparentEnergy left, ApparentEnergy right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ApparentEnergy operator *(ApparentEnergy value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ApparentEnergy operator /(ApparentEnergy value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ApparentEnergy operator *(double factor, ApparentEnergy value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ApparentEnergy left, ApparentEnergy right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ApparentEnergy number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ApparentEnergy angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ApparentEnergy other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(ApparentEnergy left, ApparentEnergy right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ApparentEnergy left, ApparentEnergy right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ApparentEnergy left, ApparentEnergy right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ApparentEnergy left, ApparentEnergy right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ApparentEnergy left, ApparentEnergy right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ApparentEnergy left, ApparentEnergy right) => left._Value >= right._Value;

    #endregion
}
