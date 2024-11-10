using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

/// <summary>
/// 
/// </summary>
public class RestDosageProviderConfiguration : DosageProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public RestConfiguration? EndPoint { get; set; }
}

