using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

public class PhaseCapability
{
    /// <summary>
    /// The AC voltage range and quantisation this source is able to provide.
    /// </summary>
    public QuantizedRange<Voltage>? AcVoltage { get; set; }

    /// <summary>
    /// The AC current range and quantisation this source is able to provide.
    /// </summary>
    public QuantizedRange<Current>? AcCurrent { get; set; }

    /// <summary>
    /// The DC voltage range and quantisation this source is able to provide.
    /// </summary>
    public QuantizedRange<Voltage>? DcVoltage { get; set; }

    /// <summary>
    /// The DC current range and quantisation this source is able to provide.
    /// </summary>
    public QuantizedRange<Current>? DcCurrent { get; set; }
}