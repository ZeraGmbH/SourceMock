using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Options;

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
        Coarse = coarse;
        Fine = fine;
    }

    /// <summary>
    /// Major calibration value between 0 and 127.
    /// </summary>
    [NotNull, Required]
    public byte Coarse
    {
        get { return _Coarse; }
        set
        {
            // Validate and remember.
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 127, nameof(value));

            _Coarse = value;
        }
    }

    private byte _Coarse;

    /// <summary>
    /// Minor calibration value between 0 and 127.
    /// </summary>
    [NotNull, Required]
    public byte Fine
    {
        get { return _Fine; }
        set
        {
            // Validate and remember.
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 127, nameof(value));

            _Fine = value;
        }
    }

    private byte _Fine;

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

    /// <inheritdoc/>
    public override int GetHashCode() => (Coarse.GetHashCode() << 4) ^ Fine.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is CalibrationPair other && other.Coarse == Coarse && other.Fine == Fine;

    /// <summary>
    /// Create a string describing this value pair.
    /// </summary>
    /// <inheritdoc/>
    public override string ToString() => $"{Coarse}:{Fine}";

    /// <summary>
    /// Reconstruct a calibration value pair from protocol data.
    /// </summary>
    /// <param name="coarse">Coarse value.</param>
    /// <param name="fine">Fine value.</param>
    /// <returns>The corresponding pair.</returns>
    public static CalibrationPair Parse(string coarse, string fine)
        => new(
           coarse.Length == 4 && coarse.StartsWith("0x") ? byte.Parse(coarse[2..], NumberStyles.HexNumber) : throw new ArgumentException($"bad calibration value: {coarse}", nameof(coarse)),
            fine.Length == 4 && fine.StartsWith("0x") ? byte.Parse(fine[2..], NumberStyles.HexNumber) : throw new ArgumentException($"bad calibration value: {fine}", nameof(fine))
        );
}
