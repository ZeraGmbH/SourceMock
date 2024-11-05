namespace BurdenApi.Models;

/// <summary>
/// Single pair of calibration values with coarse
/// and fine values. 
/// </summary>
public class CalibrationPair()
{
    /// <summary>
    /// Initialize a new pair.
    /// </summary>
    /// <param name="coarse">Major calibration between 0 and 127.</param>
    /// <param name="fine">Minor calibration between 0 and 127.</param>
    public CalibrationPair(byte coarse, byte fine) : this()
    {
        // Validate and remember.
        ArgumentOutOfRangeException.ThrowIfGreaterThan(coarse, 127, nameof(coarse));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fine, 127, nameof(fine));

        Coarse = coarse;
        Fine = fine;
    }

    /// <summary>
    /// Major calibration value between 0 and 127.
    /// </summary>
    public byte Coarse { get; private set; }

    /// <summary>
    /// Minor calibration value between 0 and 127.
    /// </summary>
    public byte Fine { get; private set; }

    /// <summary>
    /// Change the coarse calibration.
    /// </summary>
    /// <param name="increment">Set to increment calibration value.</param>
    /// <returns>New calibration pair if changes are applied or null if nothing changed.</returns>
    public CalibrationPair? ChangeCoarse(bool increment)
        => increment switch
        {
            true => Coarse < 127 ? new CalibrationPair((byte)(Coarse + 1), Fine) : null,
            false => Coarse > 0 ? new CalibrationPair((byte)(Coarse - 1), Fine) : null,
        };

    /// <summary>
    /// Change the fine calibration.
    /// </summary>
    /// <param name="increment">Set to increment calibration value.</param>
    /// <returns>New calibration pair if changes are applied or null if nothing changed.</returns>
    public CalibrationPair? ChangeFine(bool increment)
        => increment switch
        {
            true => Fine < 127 ? new CalibrationPair(Coarse, (byte)(Fine + 1)) : null,
            false => Fine > 0 ? new CalibrationPair(Coarse, (byte)(Fine - 1)) : null,
        };

    /// <summary>
    /// Create a hash code from the elementary values.
    /// </summary>
    /// <returns>Some hash code based on our values.</returns>
    public override int GetHashCode() => Coarse.GetHashCode() ^ Fine.GetHashCode();

    /// <summary>
    /// Create a string describing this value pair.
    /// </summary>
    public override string ToString() => $"{Coarse}:{Fine}";
}
