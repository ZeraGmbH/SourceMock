

using SourceApi.Model;

namespace RefMeterApi.Models;

/// <summary>
/// Report the actual values of a reference meter.
/// </summary>
public class MeasuredLoadpoint : AbstractLoadpoint<MeasuredLoadpointPhase, ElectricalQuantity>
{
    /// <summary>
    /// Order of phases, default is 123.
    /// </summary>
    public string? PhaseOrder { get; set; }

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
