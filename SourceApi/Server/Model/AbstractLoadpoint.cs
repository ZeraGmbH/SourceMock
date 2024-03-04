namespace SourceApi.Model;

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