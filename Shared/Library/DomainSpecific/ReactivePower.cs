using System.Text.Json.Serialization;


namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Rective power (in VAr) as domain specific number.
/// </summary>
public readonly struct ReactivePower(double value) : IInternalDomainSpecificNumber<ReactivePower>
{
    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static ReactivePower Create(double value) => new(value);

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "VAr";

    /// <summary>
    /// No power at all.
    /// </summary>
    public static readonly ReactivePower Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="power">Some active power.</param>
    public static explicit operator double(ReactivePower power) => power._Value;

    /// <inheritdoc/>
    public override string? ToString() => this.Format();

    #region Arithmetics

    /// <inheritdoc/>
    public static ReactivePower operator +(ReactivePower left, ReactivePower right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static ReactivePower operator -(ReactivePower left, ReactivePower right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static ReactivePower operator -(ReactivePower value) => new(-value._Value);

    /// <inheritdoc/>
    public static ReactivePower operator %(ReactivePower left, ReactivePower right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static ReactivePower operator *(ReactivePower value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static ReactivePower operator /(ReactivePower value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static ReactivePower operator *(double factor, ReactivePower value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(ReactivePower left, ReactivePower right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(ReactivePower number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ReactivePower angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(ReactivePower other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(ReactivePower left, ReactivePower right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(ReactivePower left, ReactivePower right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(ReactivePower left, ReactivePower right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(ReactivePower left, ReactivePower right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(ReactivePower left, ReactivePower right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(ReactivePower left, ReactivePower right) => left._Value >= right._Value;

    #endregion
}
