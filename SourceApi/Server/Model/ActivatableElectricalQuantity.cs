namespace SourceApi.Model;

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
///     [2] On
/// </remarks>
public class ActivatableElectricalQuantity<T> : ElectricalQuantity<T> where T : struct
{
    public bool On { get; set; }
}
