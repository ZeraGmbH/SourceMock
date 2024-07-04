using SharedLibrary.DomainSpecific;

namespace SourceApi.Model
{
    public class PhaseCapability
    {
        /// <summary>
        /// The AC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange<Voltage>? AcVoltageNGX { get; set; }

        /// <summary>
        /// The AC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange<Current>? AcCurrentNGX { get; set; }

        /// <summary>
        /// The DC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange<Voltage>? DcVoltageNGX { get; set; }

        /// <summary>
        /// The DC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange<Current>? DcCurrentNGX { get; set; }
    }
}