namespace SourceApi.Model;

public abstract class AbstractLoadpointPhase<T> where T: ElectricalQuantity, new()
{
    public T Voltage { get; set; } = new();
    public T Current { get; set; } = new();
}
