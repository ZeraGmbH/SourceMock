using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Source;

namespace ZeraDevices.Source;

/// <summary>
/// 
/// </summary>
public interface ICapabilitiesMap
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    SourceCapabilities GetCapabilitiesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltageAmplifier"></param>
    /// <param name="currentAmplifier"></param>
    /// <returns></returns>
    public SourceCapabilities GetCapabilitiesByAmplifiers(VoltageAmplifiers voltageAmplifier, CurrentAmplifiers currentAmplifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    public Voltage[] GetVoltageRangesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="voltageAmplifier"></param>
    /// <returns></returns>
    public Voltage[] GetRangesByAmplifier(VoltageAmplifiers voltageAmplifier);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelName"></param>
    /// <returns></returns>
    public Current[] GetCurrentRangesByModel(string modelName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentAmplifier"></param>
    /// <returns></returns>
    public Current[] GetRangesByAmplifier(CurrentAmplifiers currentAmplifier);
}
