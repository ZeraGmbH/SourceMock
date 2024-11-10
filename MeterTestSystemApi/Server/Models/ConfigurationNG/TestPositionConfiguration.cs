using MeterTestSystemApi.Models.ConfigurationNG.ErrorCalculator;

namespace MeterTestSystemApi.Models.ConfigurationNG;

internal class TestPositionConfiguration
{
    internal ErrorCalculatorConfiguration? ErrorCalculator { get; set; }

    internal ZIFSocketConfiguration? ZIFSocket { get; set; }
}
