using SharedLibrary.DomainSpecific;

namespace SourceApi.Model;

public abstract class AbstractLoadpointPhase<T> where T : ElectricalQuantity, new()
{
    public T Voltage { get; set; } = new();

    public T Current { get; set; } = new();
}

public abstract class AbstractLoadpointPhase<TVoltage, TCurrent>
    where TVoltage : ElectricalQuantity<Voltage>, new()
    where TCurrent : ElectricalQuantity<Current>, new()
{
    public TVoltage Voltage { get; set; } = new();

    public TCurrent Current { get; set; } = new();
}
