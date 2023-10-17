namespace WebSamDeviceApis.Model
{
    public class SourceCapabilities
    {
        public List<PhaseCapability> Phases { get; set; } = new();
        public List<FrequencyRange> FrequencyRanges { get; set; } = new();
    }
}