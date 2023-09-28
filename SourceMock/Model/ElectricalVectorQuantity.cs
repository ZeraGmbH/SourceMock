using System.ComponentModel.DataAnnotations;

namespace SourceMock.Model
{
    /// <summary>
    /// Objects of this class are to represent electical quantities typically displayed in vector form, 
    /// so primarily voltage and current.
    /// </summary>
    [Serializable]
    public class ElectricalVectorQuantity
    {
        private const double MINIMUM_ANGLE = 0;
        private const double MAXIMUM_ANGLE = 360;

        /// <summary>
        /// The root mean square value, based on the fundamental frequency.
        /// </summary>
        [Required]
        public double Rms { get; set; }

        /// <summary>
        /// The phase angle of this vector.
        /// </summary>
        [Required]
        [Range(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public double Angle { get; set; }

        /// <summary>
        /// A list of harmonics as a facor of the base amplitude. The index encodes the harmonic off by 
        /// two, e.g. the index zero contains the 2nd harmonic (e.g. 100 Hz for a fundamental of 50 Hz),
        /// index one the 3rd harmonic and so forth.
        /// </summary>
        public List<double> Harmonics { get; set; } = new();
    }
}