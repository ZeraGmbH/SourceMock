using SourceApi.Model;

namespace RefMeterApi.Models;

/// <summary>
/// Reports the actual values for a single phase of a reference meter.
/// </summary>
public class MeasuredLoadpointPhase<T> : AbstractLoadpointPhase<T> where T : ElectricalQuantity, new()
{
    /// <summary>
    /// [tbd]
    /// </summary>
    public double? PowerFactor { get; set; }

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
/// Reports the actual values for a single phase of a reference meter.
/// </summary>
public class MeasuredLoadpointPhase : MeasuredLoadpointPhase<ElectricalQuantity>
{
}