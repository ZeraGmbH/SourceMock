using MeterTestSystemApi.Models.ConfigurationNG.ErrorCalculator;

namespace MeterTestSystemApi.Models.ConfigurationNG;

/// <summary>
/// 
/// </summary>
public class TestPositionConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public ErrorCalculatorConfiguration? ErrorCalculator { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public ZIFSocketConfiguration? ZIFSocket { get; set; }
}
