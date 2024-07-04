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
    /// Add to active energies.
    /// </summary>
    /// <param name="left">One energy.</param>
    /// <param name="right">Another energy.</param>
    /// <returns>New active energy instance representing the sum of the parameters.</returns>
    public static ActiveEnergy operator +(ActiveEnergy left, ActiveEnergy right) => new(left._Value + right._Value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ActiveEnergy operator -(ActiveEnergy left, ActiveEnergy right) => new(left._Value - right._Value);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ActiveEnergy operator *(ActiveEnergy energy, double factor) => new(energy._Value * factor);

    /// <summary>
    /// Scale energy by a factor.
    /// </summary>
    /// <param name="energy">Some energy.</param>
    /// <param name="factor">Factor to apply to the energy.</param>
    /// <returns>New energy with scaled value.</returns>
    public static ActiveEnergy operator *(double factor, ActiveEnergy energy) => energy * factor;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator <(ActiveEnergy left, ActiveEnergy right) => left._Value < right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator >(ActiveEnergy left, ActiveEnergy right) => left._Value > right._Value;
}
