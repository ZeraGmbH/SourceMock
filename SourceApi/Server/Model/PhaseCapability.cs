namespace SourceApi.Model
{
    public class PhaseCapability
    {
        /// <summary>
        /// The voltage range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange Voltage { get; set; } = new();

        /// <summary>
        /// The current range and quantisation this source is able to provide.
        /// </summary>
        public QuantizedRange Current { get; set; } = new();
    }
}