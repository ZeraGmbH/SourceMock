using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BurdenApi.Models;

/// <summary>
/// Full calibration information.
/// </summary>
/// <param name="resistive">Resistive calibration.</param>
/// <param name="inductive">Inductive calibration.</param>
public class Calibration(CalibrationPair resistive, CalibrationPair inductive)
{
    /// <summary>
    /// For serialisation only.
    /// </summary>
    public Calibration() : this(new(), new()) { }

    /// <summary>
    /// Calibration on the resistive load.
    /// </summary>
    [NotNull, Required]
    public CalibrationPair Resistive { get; set; } = resistive;

    /// <summary>
    /// Calibration on the inductive load.
    /// </summary>
    [NotNull, Required]
    public CalibrationPair Inductive { get; set; } = inductive;

    /// <inheritdoc/>
    public override int GetHashCode() => (Resistive.GetHashCode() << 4) ^ Inductive.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Calibration other && other.Resistive.Equals(Resistive) && other.Inductive.Equals(Inductive);

    /// <inheritdoc/>
    public override string ToString() => $"r({Resistive})/i({Inductive})";

    /// <summary>
    /// Reconstruct a calibration from the raw protocol values.
    /// </summary>
    /// <param name="values">Full value information.</param>
    /// <returns>null if the calibration has not been done.</returns>
    public static Tuple<Calibration, string>? Parse(string values)
    {
        // Do a core analysis of the string and check for non existing calibrations.
        var split = values.Split(";");

        if (split.Length < 1) throw new ArgumentException($"bad calibration, value empty: {values}", nameof(values));

        if (split.Length < 2) return split[0] == "0" ? null : throw new ArgumentException($"bad calibration, expected 0: {values}", nameof(values));

        if (split.Length < 6) throw new ArgumentException($"bad calibration, expected at least six parts: {values}", nameof(values));

        if (split[0] != "1") throw new ArgumentException($"bad calibration, wrong activation flag: {values}", nameof(values));

        // Got a calibration.
        return Tuple.Create(
            new Calibration(CalibrationPair.Parse(split[1], split[2]), CalibrationPair.Parse(split[3], split[4])),
            string.Join(";", split.Skip(5))
        );
    }
}
