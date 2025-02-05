

using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;

namespace RefMeterApi.Models;

/// <summary>
/// Report the actual values of a reference meter.
/// </summary>
public class MeasuredLoadpoint<TPhase, TVoltage, TCurrent> : AbstractLoadpoint<TPhase, TVoltage, TCurrent>
   where TPhase : AbstractLoadpointPhase<TVoltage, TCurrent>
   where TVoltage : ElectricalQuantity<Voltage>, new()
   where TCurrent : ElectricalQuantity<Current>, new()
{
    /// <summary>
    /// Order of phases, default is 123.
    /// </summary>
    public string? PhaseOrder { get; set; }

    /// <summary>
    /// Total active power (W).
    /// </summary>
    public ActivePower? ActivePower { get; set; }

    /// <summary>
    /// Total reactive power (VAr).
    /// </summary>
    public ReactivePower? ReactivePower { get; set; }

    /// <summary>
    /// Total apperent power (VA).
    /// </summary>
    public ApparentPower? ApparentPower { get; set; }

    /// <summary>
    /// Frequency.
    /// </summary>
    public Frequency? Frequency { get; set; }
}

/// <summary>
/// Report the actual values of a reference meter.
/// </summary>
public class MeasuredLoadpoint : MeasuredLoadpoint<MeasuredLoadpointPhase, ElectricalQuantity<Voltage>, ElectricalQuantity<Current>>
{
}
