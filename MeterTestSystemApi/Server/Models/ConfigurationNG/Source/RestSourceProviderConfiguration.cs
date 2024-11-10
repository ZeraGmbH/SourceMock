using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

/// <summary>
/// 
/// </summary>
public class RestSourceProviderConfiguration : SourceProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public RestConfiguration? EndPoint { get; set; }
}
