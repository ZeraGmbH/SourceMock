using System.ComponentModel.DataAnnotations;

namespace SourceMock.Model
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
        /// A list of voltages for the different phases in ascending order.
        /// </summary>
        public List<ElectricalVectorQuantity> Voltages { get; set; } = new();

        /// <summary>
        /// Determines whether or not the voltages neutral conductor is to be connected.
        /// </summary>
        public bool VoltageNeutralConnected { get; set; }

        /// <summary>
        /// A list of currents for the differnt phases in ascending order.
        /// </summary>
        public List<ElectricalVectorQuantity> Currents { get; set; } = new();

        /// <summary>
        /// The frequency.
        /// </summary>
        public Frequency Frequency { get; set; } = new();

        /// <summary>
        /// The auxilliary voltage, null if none exits.
        /// </summary>
        public ElectricalVectorQuantity? AuxilliaryVoltage { get; set; }

        /// <summary>
        /// The phase angle of the auxilliary voltage.
        /// </summary>
        [Range(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public double PhaseAngleAuxilliaryVoltage { get; set; }
    }
}
