using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.DomainSpecific;

namespace SourceApi.Model;

public class FrequencyRange : QuantizedRange<Frequency>
{
    public FrequencyMode Mode { get; set; }

    public FrequencyRange(Frequency lowerEndpoint, Frequency upperEndpoint, Frequency quantisationDistance, FrequencyMode mode) :
        base(lowerEndpoint, upperEndpoint, quantisationDistance)
    {
        Mode = mode;
    }

    public FrequencyRange()
    {
    }
}