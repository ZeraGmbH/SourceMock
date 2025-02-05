using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models;

namespace RefMeterApi.Models;

/// <summary>
/// Reports the actual values for a single phase of a reference meter.
/// </summary>
public class MeasuredLoadpointPhase<TVoltage, TCurrent> : AbstractLoadpointPhase<TVoltage, TCurrent>
    where TVoltage : ElectricalQuantity<Voltage>, new()
    where TCurrent : ElectricalQuantity<Current>, new()
{
    /// <summary>
    /// [tbd]
    /// </summary>
    public PowerFactor? PowerFactor { get; set; }

    /// <summary>
    /// Active power (W).
    /// </summary>
    public ActivePower? ActivePower { get; set; }

    /// <summary>
    /// Reactive power (VAr).
    /// </summary>
    public ReactivePower? ReactivePower { get; set; }

    /// <summary>
    /// Apparent power (VA).
    /// </summary>
    public ApparentPower? ApparentPower { get; set; }
}

/// <summary>
/// Reports the actual values for a single phase of a reference meter.
/// </summary>
public class MeasuredLoadpointPhase : MeasuredLoadpointPhase<ElectricalQuantity<Voltage>, ElectricalQuantity<Current>>
{
}