using ZERA.WebSam.Shared.DomainSpecific;

namespace SourceApi.Model;

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
/// </remarks>
public class ElectricalQuantity<T> where T : struct, IDomainSpecificNumber<T>
{
    /// <summary>
    /// 
    /// </summary>
    public T? DcComponent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ElectricalVectorQuantity<T>? AcComponent { get; set; }
}
