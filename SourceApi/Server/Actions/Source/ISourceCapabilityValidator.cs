using SourceApi.Model;

namespace SourceApi.Actions.Source;

/// <summary>
/// 
/// </summary>
public interface ISourceCapabilityValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loadpoint"></param>
    /// <param name="capabilities"></param>
    /// <returns></returns>
    public SourceApiErrorCodes IsValid(TargetLoadpoint loadpoint, SourceCapabilities capabilities);
}
