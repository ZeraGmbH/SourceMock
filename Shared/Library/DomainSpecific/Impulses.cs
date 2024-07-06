using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Impulses (a constant number) as domain specific number.
/// </summary>
public readonly struct Impulses(double value) : IInternalDomainSpecificNumber<Impulses>
{
    /// <summary>
    /// The real value is always represented as a long.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "";

    /// <summary>
    /// Create from integral value.
    /// </summary>
    /// <param name="value">Number of impulses.</param>
    public Impulses(long value) : this((double)value) { }

    /// <summary>
    /// No Impulses at all.
    /// </summary>
    public static readonly Impulses Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Impulses">Some active Impulses.</param>
    public static explicit operator long(Impulses Impulses) => (long)Impulses._Value;

    /// <inheritdoc/>
    public static explicit operator double(Impulses Impulses) => Impulses._Value;

    /// <summary>
    /// Devide impulses with meterConstant to get ActiveEnergy
    /// </summary>
    /// <param name="impulses">Some Impulses.</param>
    /// <param name="meterConstant">MeterConstant.</param>
    /// <returns>New ActiveEnergy.</returns>
    public static ActiveEnergy operator /(Impulses impulses, MeterConstant meterConstant) => new(impulses._Value / (double)meterConstant * 1000);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Impulses Ceil() => new((long)Math.Ceiling(_Value));

    #region Arithmetics

    /// <inheritdoc/>
    public static Impulses operator +(Impulses left, Impulses right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Impulses operator -(Impulses left, Impulses right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Impulses operator -(Impulses value) => new(-value._Value);

    /// <inheritdoc/>
    public static Impulses operator %(Impulses left, Impulses right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Impulses operator *(Impulses value, double factor) => new(value._Value * factor);

    /// <inheritdoc/>
    public static Impulses operator /(Impulses value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Impulses operator *(double factor, Impulses value) => new(factor * value._Value);

    /// <inheritdoc/>
    public static double operator /(Impulses left, Impulses right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Impulses number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Impulses angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Impulses other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Impulses left, Impulses right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Impulses left, Impulses right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Impulses left, Impulses right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Impulses left, Impulses right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Impulses left, Impulses right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Impulses left, Impulses right) => left._Value >= right._Value;

    #endregion
}
