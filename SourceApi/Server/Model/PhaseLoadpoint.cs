namespace SourceApi.Model
{
    public class PhaseLoadpoint
    {
        public ElectricalQuantity Voltage { get; set; } = new();
        public ElectricalQuantity Current { get; set; } = new();
    }
}