namespace SourceApi.Model
{
    /// <summary>
    /// Describes a specific loadpoint.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: order of properties is important to use with blockly. 
    /// The dedicated loadpoint editor expects the following order:
    ///     [0] Phases
    ///     [1] VoltageNeutralConnected
    ///     [2] Frequency
    /// </remarks>
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
