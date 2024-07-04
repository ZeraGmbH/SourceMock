using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

public abstract class AbstractLoadpointPhase<TVoltage, TCurrent>
    where TVoltage : ElectricalQuantity<Voltage>, new()
    where TCurrent : ElectricalQuantity<Current>, new()
{
    public TVoltage Voltage { get; set; } = new();

    public TCurrent Current { get; set; } = new();
}
