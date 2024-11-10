using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;

/// <summary>
/// 
/// </summary>
public class RestMeterTestSystemProviderConfiguration : MeterTestSystemProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public RestConfiguration? EndPoint { get; set; }
}
