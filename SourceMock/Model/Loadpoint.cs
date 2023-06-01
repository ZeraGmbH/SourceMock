using SourceMock.Actions.LoadpointValidator;
using System.ComponentModel.DataAnnotations;

namespace SourceMock.Model
{
    public class Loadpoint
    {
        private const double MINIMUM_ANGLE = 0;
        private const double MAXIMUM_ANGLE = 360;

        public IEnumerable<double> Voltages { get; set; } = new List<double>();
        public IEnumerable<double> Currents { get; set; } = new List<double>();

        [RangeEnumerable(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public IEnumerable<double> PhaseAnglesVoltage { get; set; } = new List<double>();

        [RangeEnumerable(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public IEnumerable<double> PhaseAnglesCurrent { get; set; } = new List<double>();
    }
}
