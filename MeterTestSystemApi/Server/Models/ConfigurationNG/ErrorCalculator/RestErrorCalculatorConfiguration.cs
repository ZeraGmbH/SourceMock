using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.ErrorCalculator;

internal class RestErrorCalculatorConfiguration : ErrorCalculatorConfiguration
{
    internal RestConfiguration? EndPoint { get; set; }
}
