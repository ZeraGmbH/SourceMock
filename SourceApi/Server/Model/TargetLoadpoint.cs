namespace SourceApi.Model
{
    /// <summary>
    /// Describes a specific loadpoint.
    /// </summary>
    [Serializable]
    public class TargetLoadpoint : AbstractLoadpoint<TargetLoadpointPhase, ActivatableElectricalQuantity>
    {
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
