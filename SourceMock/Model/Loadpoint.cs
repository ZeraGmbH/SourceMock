using System.ComponentModel.DataAnnotations;

namespace SourceMock.Model
{
    public class Loadpoint
    {
        private const double MINIMUM_ANGLE = 0;
        private const double MAXIMUM_ANGLE = 360;

        [Range(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public double PhaseAngleVoltage { get; set; }
    }
}
