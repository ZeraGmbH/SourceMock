namespace SourceApi.Model;

/// <remarks>
/// IMPORTANT: order of properties is important to use with blockly. 
/// The dedicated loadpoint editor expects the following order:
///     [0] Phases
/// </remarks>
[Serializable]
public abstract class AbstractLoadpoint<TPhase, TQuantity>
    where TPhase : AbstractLoadpointPhase<TQuantity>
    where TQuantity : ElectricalQuantity, new()
{
    /// <summary>
    /// The phases of this loadpoint.
    /// </summary>
    public List<TPhase> Phases { get; set; } = [];
}