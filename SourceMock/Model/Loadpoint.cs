using System.ComponentModel.DataAnnotations;

namespace SourceMock.Model
{
    public class Loadpoint
    {
        private const Double MINIMUM_ANGLE = 0;
        private const Double MAXIMUM_ANGLE = 360;

        [Range(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public Double PhaseAngleVoltage { get; set; }
    }
}
