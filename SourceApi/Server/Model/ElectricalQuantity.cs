using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
/// </remarks>
public class ElectricalQuantity
{
    public double? DcComponent { get; set; }

    public ElectricalVectorQuantity? AcComponent { get; set; }
}

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
/// </remarks>
public abstract class ElectricalCurrentQuantity<T>
{
    public T? DcComponent { get; set; }

    public ElectricalVectorQuantity<T>? AcComponent { get; set; }
}

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
/// </remarks>
public class ElectricalCurrentQuantity : ElectricalCurrentQuantity<Current>
{
}

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] DcComponent
///     [1] AcComponent
/// </remarks>
public class ElectricalVoltageQuantity : ElectricalCurrentQuantity<Voltage>
{
}