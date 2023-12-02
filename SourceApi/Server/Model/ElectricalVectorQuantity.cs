using System.ComponentModel.DataAnnotations;

namespace SourceApi.Model
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
        /// The root mean square value, based on the fundamental frequency. Always without any SI-Prefix, so depending on the quantity descibed in V, A, W, VA, or var.
        /// </summary>
        [Required]
        public double Rms { get; set; }

        /// <summary>
        /// The phase angle of this vector in degrees.
        /// </summary>
        [Required]
        [Range(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
        public double Angle { get; set; }

        public bool On { get; set; }
    }
}