namespace SourceApi.Model
{
    public class PhaseLoadpoint
    {
        public ActivatableElectricalQuantity Voltage { get; set; } = new();
        public ActivatableElectricalQuantity Current { get; set; } = new();
    }
}