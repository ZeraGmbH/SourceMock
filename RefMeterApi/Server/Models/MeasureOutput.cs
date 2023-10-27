namespace RefMeterApi.Models;

/// <summary>
/// Reports the actual values for a single phase of a reference meter.
/// </summary>
public class MeasureOutputPhase
{
    /// <summary>
    /// Voltage (V).
    /// </summary>
    public double? Voltage { get; set; }

    /// <summary>
    /// Current (A).
    /// </summary>
    public double? Current { get; set; }

    /// <summary>
    /// Phase angle of the voltage.
    /// </summary>
    public double? AngleVoltage { get; set; }

    /// <summary>
    /// Phase angle of the current.
    /// </summary>
    public double? AngleCurrent { get; set; }

    /// <summary>
    /// [tbd]
    /// </summary>
    public double? Angle { get; set; }

    /// <summary>
    /// Active power (W).
    /// </summary>
    public double? ActivePower { get; set; }

    /// <summary>
    /// Reactive power (VAr).
    /// </summary>
    public double? ReactivePower { get; set; }

    /// <summary>
    /// Apparent power (VA).
    /// </summary>
    public double? ApparentPower { get; set; }
}

/// <summary>
/// Report the actual values of a reference meter.
/// </summary>
public class MeasureOutput
{
    /// <summary>
    /// Order of phases, default is 123.
    /// </summary>
    public int? PhaseOrder { get; set; }

    /// <summary>
    /// Individual values for each phase.
    /// </summary>
    public List<MeasureOutputPhase> Phases { get; set; } = new();

    /// <summary>
    /// Total active power (W).
    /// </summary>
    public double? ActivePower { get; set; }

    /// <summary>
    /// Total reactive power (VAr).
    /// </summary>
    public double? ReactivePower { get; set; }

    /// <summary>
    /// Total apperent power (VA).
    /// </summary>
    public double? ApparentPower { get; set; }

    /// <summary>
    /// Frequency.
    /// </summary>
    public double? Frequency { get; set; }
}
