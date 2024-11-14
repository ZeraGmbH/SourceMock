using ZERA.WebSam.Shared.DomainSpecific;

namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
public class RefMeterStatus
{
    /// <summary>
    /// Actual input channel (e.g. UB=250 or K1=10V) 
    /// </summary>
    public Voltage? VoltageRange { get; set; }

    /// <summary>
    /// Actual input channel (e.g. IB=10 or K2=0.02A(? -> I2=0.02A?))
    /// </summary>
    public Current? CurrentRange { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public MeasurementModes? MeasurementMode { get; set; }

    /// <summary>
    /// PLL-Reference
    /// </summary>
    public PllChannel? PllChannel { get; set; }
}