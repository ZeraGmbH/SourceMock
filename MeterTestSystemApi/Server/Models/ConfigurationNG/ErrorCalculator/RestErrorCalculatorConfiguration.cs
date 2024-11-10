using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.ErrorCalculator;

/// <summary>
/// 
/// </summary>
public class RestErrorCalculatorConfiguration : ErrorCalculatorConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public RestConfiguration? EndPoint { get; set; }
}
