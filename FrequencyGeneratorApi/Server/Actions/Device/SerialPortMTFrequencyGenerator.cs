using FrequencyGeneratorApi.Models;

namespace FrequencyGeneratorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public class SerialPortMTFrequencyGenerator : IFrequencyGenerator
{
    /// <inheritdoc/>
    public Task<FrequencyGeneratorCapabilities> GetCapabilities() => Task.FromResult<FrequencyGeneratorCapabilities>(null!);

    /// <inheritdoc/>
    public Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter)
    {
        throw new NotImplementedException();
    }
}
