using FrequencyGeneratorApi.Models;

namespace FrequencyGeneratorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface IFrequencyGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<FrequencyGeneratorCapabilities> GetCapabilities();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltage"></param>
    /// <param name="current"></param>
    /// <param name="referenceMeter"></param>
    /// <returns></returns>
    Task SetAmplifiersAndReferenceMeter(VoltageAmplifiers voltage, CurrentAmplifiers current, ReferenceMeters referenceMeter);
}
