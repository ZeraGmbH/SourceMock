namespace WebSamDeviceApis.Model
{
    public class PhaseLoadpoint
    {
        public ElectricalVectorQuantity Voltage { get; set; } = new();
        public ElectricalVectorQuantity Current { get; set; } = new();
    }
}