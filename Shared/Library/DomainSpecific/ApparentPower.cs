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
    /// Add to apparent power.
    /// </summary>
    /// <param name="left">One power.</param>
    /// <param name="right">Another power.</param>
    /// <returns>New apparent power instance representing the sum of the parameters.</returns>
    public static ApparentPower operator +(ApparentPower left, ApparentPower right) => new(left._Value + right._Value);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ApparentPower operator *(ApparentPower power, double factor) => new(power._Value * factor);

    /// <summary>
    /// Scale power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ApparentPower operator *(double factor, ApparentPower power) => new(factor * power._Value);

    /// <summary>
    /// Divide power by a factor.
    /// </summary>
    /// <param name="power">Some power.</param>
    /// <param name="factor">Factor to apply to the power.</param>
    /// <returns>New power with scaled value.</returns>
    public static ApparentPower operator /(ApparentPower power, double factor) => new(power._Value * factor);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static double operator /(ApparentPower left, ApparentPower right) => left._Value / right._Value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static ApparentPower operator -(ApparentPower left, ApparentPower right) => new(left._Value - right._Value);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(ApparentPower other) => _Value.CompareTo(other._Value);

    #region Comparable

    /// <summary>
    /// See if the number is exactly zero.
    /// </summary>
    /// <param name="number">Some number.</param>
    /// <returns>Set if number is zero.</returns>
    public static bool operator !(ApparentPower number) => number._Value == 0;

    /// <summary>
    /// Compare with any other object.
    /// </summary>
    /// <param name="obj">Some other object.</param>
    /// <returns>Set if this number is identical to the other object.</returns>
    public override bool Equals(object? obj) => obj is ApparentPower angle && _Value == angle._Value;

    /// <summary>
    /// Get a hashcode.
    /// </summary>
    /// <returns>Hashcode for this number.</returns>
    public override int GetHashCode() => _Value.GetHashCode();

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are exactly identical.</returns>
    public static bool operator ==(ApparentPower left, ApparentPower right) => left._Value == right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the numbers are not exactly identical.</returns>
    public static bool operator !=(ApparentPower left, ApparentPower right) => left._Value != right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is less than the second number.</returns>
    public static bool operator <(ApparentPower left, ApparentPower right) => left._Value < right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not greater than the second number.</returns>
    public static bool operator <=(ApparentPower left, ApparentPower right) => left._Value <= right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is greater than the second number.</returns>
    public static bool operator >(ApparentPower left, ApparentPower right) => left._Value > right._Value;

    /// <summary>
    /// Compare two numbers.
    /// </summary>
    /// <param name="left">First number.</param>
    /// <param name="right">Second number.</param>
    /// <returns>Set if the first number is not less than the second number.</returns>
    public static bool operator >=(ApparentPower left, ApparentPower right) => left._Value >= right._Value;

    #endregion
}
