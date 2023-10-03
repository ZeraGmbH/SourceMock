
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
        /// Ranges of voltages supported by this source in volt. Empty or null for current-only sources.
        /// </summary>
        public List<Range>? VoltageRanges { get; set; } = new();

        /// <summary>
        /// Ranges of currents supported by this source in ampere.
        /// </summary>
        public List<Range> CurrentRanges { get; set; } = new();

        /// <summary>
        /// The highest harmonic supported by this source. 1 if not supported at all.
        /// </summary>
        [Min(1)]
        public int MaxHarmonic { get; set; }
    }
}