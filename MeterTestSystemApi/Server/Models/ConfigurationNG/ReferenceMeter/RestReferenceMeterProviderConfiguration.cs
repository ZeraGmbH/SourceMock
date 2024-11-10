using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

/// <summary>
/// 
/// </summary>
public class RestReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public RestConfiguration? EndPoint { get; set; }
}
