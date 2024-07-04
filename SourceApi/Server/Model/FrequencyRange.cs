namespace SourceApi.Model;

using FrequencyDSN = SharedLibrary.DomainSpecific.Frequency;

public class FrequencyRange : QuantizedRange<FrequencyDSN>
{
    public FrequencyMode Mode { get; set; }

    public FrequencyRange(FrequencyDSN lowerEndpoint, FrequencyDSN upperEndpoint, FrequencyDSN quantisationDistance, FrequencyMode mode) :
        base(lowerEndpoint, upperEndpoint, quantisationDistance)
    {
        Mode = mode;
    }

    public FrequencyRange()
    {
    }
}