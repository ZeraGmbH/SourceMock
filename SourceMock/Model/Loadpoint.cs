using SourceMock.Actions.LoadpointValidator;

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
        /// A list of voltages for the different phases in the order L1, L2, L3.
        /// </summary>
        public IEnumerable<double> Voltages { get; set; } = new List<double>();

        /// <summary>
        /// A list of currents for the differnt phases in the order L1, L2, L3.
        /// </summary>
        public IEnumerable<double> Currents { get; set; } = new List<double>();

        /// <summary>
        /// The frequency.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// The angles of the voltages in the differnt phases in the order L1, L2, L3.
        /// </summary>
        [RangeEnumerable(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public IEnumerable<double> PhaseAnglesVoltage { get; set; } = new List<double>();

        /// <summary>
        /// The angle of the currents in the differnt phases in the order L1, L2, L3.
        /// </summary>
        [RangeEnumerable(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public IEnumerable<double> PhaseAnglesCurrent { get; set; } = new List<double>();
    }
}
