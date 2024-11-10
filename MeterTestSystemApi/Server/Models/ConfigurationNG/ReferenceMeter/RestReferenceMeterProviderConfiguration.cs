using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

internal class RestReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    internal RestConfiguration? EndPoint { get; set; }
}
