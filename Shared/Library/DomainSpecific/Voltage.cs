using System.Text.Json.Serialization;

namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Voltage (V) as domain specific number.
/// </summary>
public readonly struct Voltage(double value) : IInternalDomainSpecificNumber<Voltage>
{
    /// <summary>
    /// Create voltage 0.
    /// </summary>
    public Voltage() : this(0) { }

    /// <summary>
    /// Helper holding 0 volts.
    /// </summary>
    public static readonly Voltage Zero = new();

    /// <inheritdoc/>
    public double GetValue() => _Value;

    /// <inheritdoc/>
    public static Voltage Create(double value) => new(value);

    /// <summary>
    /// The real value is always represented as a double.
    /// </summary>
    private readonly double _Value = value;

    /// <inheritdoc/>
    [JsonIgnore]
    public readonly string Unit => "V";

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="Voltage">Some Voltage.</param>
    public static explicit operator double(Voltage Voltage) => Voltage._Value;

    /// <inheritdoc/>
    public override string? ToString() => this.Format();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <param name="voltage"></param>
    /// <returns></returns>
    public static ApparentPower operator *(Voltage voltage, Current current) => new(voltage._Value * (double)current);

    #region Arithmetics

    /// <inheritdoc/>
    public static Voltage operator +(Voltage left, Voltage right) => new(left._Value + right._Value);

    /// <inheritdoc/>
    public static Voltage operator -(Voltage left, Voltage right) => new(left._Value - right._Value);

    /// <inheritdoc/>
    public static Voltage operator -(Voltage value) => new(-value._Value);

    /// <inheritdoc/>
    public static Voltage operator %(Voltage left, Voltage right) => new(left._Value % right._Value);

    /// <inheritdoc/>
    public static Voltage operator *(Voltage Voltage, double factor) => new(Voltage._Value * factor);

    /// <inheritdoc/>
    public static Voltage operator /(Voltage value, double factor) => new(value._Value / factor);

    /// <inheritdoc/>
    public static Voltage operator *(double factor, Voltage voltage) => new(factor * voltage._Value);

    /// <inheritdoc/>
    public static double operator /(Voltage left, Voltage right) => left._Value / right._Value;

    #endregion

    #region Comparable

    /// <inheritdoc/>
    public static bool operator !(Voltage number) => number._Value == 0;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Voltage angle && _Value == angle._Value;

    /// <inheritdoc/>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <inheritdoc/>
    public int CompareTo(Voltage other) => _Value.CompareTo(other._Value);

    /// <inheritdoc/>
    public static bool operator ==(Voltage left, Voltage right) => left._Value == right._Value;

    /// <inheritdoc/>
    public static bool operator !=(Voltage left, Voltage right) => left._Value != right._Value;

    /// <inheritdoc/>
    public static bool operator <(Voltage left, Voltage right) => left._Value < right._Value;

    /// <inheritdoc/>
    public static bool operator <=(Voltage left, Voltage right) => left._Value <= right._Value;

    /// <inheritdoc/>
    public static bool operator >(Voltage left, Voltage right) => left._Value > right._Value;

    /// <inheritdoc/>
    public static bool operator >=(Voltage left, Voltage right) => left._Value >= right._Value;

    #endregion
}
