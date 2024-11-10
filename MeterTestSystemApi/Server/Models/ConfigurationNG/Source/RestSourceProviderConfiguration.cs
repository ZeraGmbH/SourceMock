using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

internal class RestSourceProviderConfiguration : SourceProviderConfiguration
{
    internal RestConfiguration? EndPoint { get; set; }
}
