namespace SourceApi.Model
{
    public class PhaseCapability
    {
        /// <summary>
        /// The AC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange? AcVoltage { get; set; }

        /// <summary>
        /// The AC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange? AcCurrent { get; set; }

        /// <summary>
        /// The DC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange? DcVoltage { get; set; }

        /// <summary>
        /// The DC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange? DcCurrent { get; set; }
    }
}