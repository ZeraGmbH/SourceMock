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
    public CalibrationPair Resistive { get; private set; } = resistive;

    /// <summary>
    /// Calibration on the inductive load.
    /// </summary>
    public CalibrationPair Inductive { get; private set; } = inductive;

    /// <summary>
    /// Create a hash code for this calibration.
    /// </summary>
    /// <returns>Hash code for the calibration.</returns>
    public override int GetHashCode() => Resistive.GetHashCode() ^ Inductive.GetHashCode();

    /// <summary>
    /// Create a descripte string for the calibration.
    /// </summary>
    public override string ToString() => $"r({Resistive})/i({Inductive})";
}
