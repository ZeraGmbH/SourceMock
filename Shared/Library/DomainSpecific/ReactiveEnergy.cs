using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Rective energy (in VArh) as domain specific number.
/// </summary>
public readonly struct ReactiveEnergy(double value) : IInternalDomainSpecificNumber<ReactiveEnergy>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static ReactiveEnergy Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VArh";

    /// <summary>
    /// No energy at all.
    /// </summary>
    public static readonly ReactiveEnergy Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="energy">Some active energy.</param>
    public static explicit operator double(ReactiveEnergy energy) => energy._Value;

    /// <inheritdoc/>
    public override string? ToString() => this.Format();

    #region Arithmetics

    /// <inheritdoc/>
    public static ReactiveEnergy operator +(ReactiveEnergy left, ReactiveEnergy right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ReactiveEnergy operator -(ReactiveEnergy left, ReactiveEnergy right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ReactiveEnergy operator -(ReactiveEnergy value) => new(-value._Value);

    /// <inheritdoc/>
    public static ReactiveEnergy operator %(ReactiveEnergy left, ReactiveEnergy right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ReactiveEnergy operator *(ReactiveEnergy value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ReactiveEnergy operator /(ReactiveEnergy value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ReactiveEnergy operator *(double factor, ReactiveEnergy value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ReactiveEnergy left, ReactiveEnergy right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ReactiveEnergy number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ReactiveEnergy angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ReactiveEnergy other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(ReactiveEnergy left, ReactiveEnergy right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ReactiveEnergy left, ReactiveEnergy right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ReactiveEnergy left, ReactiveEnergy right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ReactiveEnergy left, ReactiveEnergy right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ReactiveEnergy left, ReactiveEnergy right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ReactiveEnergy left, ReactiveEnergy right) => left._Value >= right._Value;

    #endregion
}
