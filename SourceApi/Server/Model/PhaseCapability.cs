namespace SourceApi.Model
{
    public class PhaseCapability
    {
        /// <summary>
        /// The AC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange AcVoltage { get; set; } = new();

        /// <summary>
        /// The AC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange AcCurrent { get; set; } = new();

        /// <summary>
        /// The DC voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange DcVoltage { get; set; } = new();

        /// <summary>
        /// The DC current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange DcCurrent { get; set; } = new();
    }
}