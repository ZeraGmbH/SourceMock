
using SourceMock.Actions.LoadpointValidator;

namespace SourceMock.Model
{
    public class SourceCapabilities
    {
        /// <summary>
        /// The number of phases this source supports.
        /// </summary>
        [Min(1)]
        public int NumberOfPhases { get; set; }

        /// <summary>
        /// Highest voltage (phase to ground) supported by this source in volt. 0 for current-only sources.
        /// </summary>
        [Min(0)]
        public double MaxVoltage { get; set; }

        /// <summary>
        /// Highest current (per phase) supported by this source in ampere.
        /// </summary>
        [Min(0)]
        public double MaxCurrent { get; set; }

        /// <summary>
        /// The highest harmonic supported by this source. 1 if not supported at all.
        /// </summary>
        [Min(1)]
        public int MaxHarmonic { get; set; }
    }
}