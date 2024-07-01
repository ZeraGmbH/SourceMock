namespace SourceApi.Model;

public abstract class AbstractLoadpointPhase<T> where T : ElectricalQuantity, new()
{
    public T Voltage { get; set; } = new();

    public T Current { get; set; } = new();
}

public abstract class AbstractLoadpointPhase<TVoltage, TCurrent>
    where TVoltage : ElectricalVoltageQuantity, new()
    where TCurrent : ElectricalCurrentQuantity, new()
{
    public TVoltage Voltage { get; set; } = new();

    public TCurrent Current { get; set; } = new();
}
