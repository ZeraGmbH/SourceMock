namespace SourceApi.Model
{
    /// <summary>
    /// Describes a specific loadpoint.
    /// </summary>
    [Serializable]
    public class Loadpoint
    {
        private const double MINIMUM_ANGLE = 0;
        private const double MAXIMUM_ANGLE = 360;

        /// <summary>
        /// The phases of this loadpoint.
        /// </summary>
        public List<PhaseLoadpoint> Phases { get; set; } = [];

        /// <summary>
        /// Determines whether or not the voltages neutral conductor is to be connected.
        /// </summary>
        public bool VoltageNeutralConnected { get; set; }

        /// <summary>
        /// The frequency.
        /// </summary>
        public Frequency Frequency { get; set; } = new();
    }
}
