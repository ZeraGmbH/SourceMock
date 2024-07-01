namespace SharedLibrary.DomainSpecific;

/// <summary>
/// Time (in s) as domain specific number.
/// </summary>
public class Time(double value) : DomainSpecificNumber(value)
{
    /// <inheritdoc/>
    public override string Unit => "s";

    /// <summary>
    /// No time at all.
    /// </summary>
    public static readonly Time Zero = new(0);

    /// <summary>
    /// Only explicit casting out the pure number is allowed.
    /// </summary>
    /// <param name="time">Some active time.</param>
    public static explicit operator double(Time time) => time.Value;

    /// <summary>
    /// Add to time.
    /// </summary>
    /// <param name="left">One time.</param>
    /// <param name="right">Another time.</param>
    /// <returns>New time instance representing the sum of the parameters.</returns>
    public static Time operator +(Time left, Time right) => new(left.Value + right.Value);

    /// <summary>
    /// Scale time by a factor.
    /// </summary>
    /// <param name="time">Some time.</param>
    /// <param name="factor">Factor to apply to the time.</param>
    /// <returns>New time with scaled value.</returns>
    public static Time operator *(Time time, double factor) => new(time.Value * factor);

    /// <summary>
    /// Scale time by a factor.
    /// </summary>
    /// <param name="time">Some time.</param>
    /// <param name="factor">Factor to apply to the time.</param>
    /// <returns>New time with scaled value.</returns>
    public static Time operator *(double factor, Time time) => new(factor * time.Value);
}
