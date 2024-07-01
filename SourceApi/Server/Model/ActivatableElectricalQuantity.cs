namespace SourceApi.Model;

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
///     [2] On
/// </remarks>
public class ActivatableElectricalQuantity : ElectricalQuantity
{
    public bool On { get; set; }
}

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
///     [2] On
/// </remarks>
public class ActivatableElectricalVoltageQuantity : ElectricalVoltageQuantity
{
    public bool On { get; set; }
}

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
///     [2] On
/// </remarks>
public class ActivatableElectricalCurrentQuantity : ElectricalCurrentQuantity
{
    public bool On { get; set; }
}