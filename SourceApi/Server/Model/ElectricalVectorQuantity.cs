using System.ComponentModel.DataAnnotations;
using ZERA.WebSam.Shared.DomainSpecific;

namespace SourceApi.Model;

/// <summary>
/// Objects of this class are to represent electical quantities typically displayed in vector form, 
/// so primarily voltage and current.
/// </summary>
/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] Rms
///     [1] Angle
/// </remarks>
[Serializable]
public class ElectricalVectorQuantity<T> where T : struct, IDomainSpecificNumber<T>
{
    private const double MINIMUM_ANGLE = 0;
    private const double MAXIMUM_ANGLE = 360;

    /// <summary>
    /// The root mean square value, based on the fundamental frequency. Always without any SI-Prefix, so depending on the quantity descibed in V, A, W, VA, or var.
    /// </summary>
    [Required]
    public T Rms { get; set; }

    /// <summary>
    /// The phase angle of this vector in degrees.
    /// </summary>
    [Required]
    [DSNRange(MINIMUM_ANGLE, MAXIMUM_ANGLE)]
    public Angle Angle { get; set; }
}
