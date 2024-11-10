using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;

internal class RestMeterTestSystemProviderConfiguration : MeterTestSystemProviderConfiguration
{
    internal RestConfiguration? EndPoint { get; set; }
}
