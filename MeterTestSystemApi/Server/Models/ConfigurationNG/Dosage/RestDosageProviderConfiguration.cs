using ZERA.WebSam.Shared.Models;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

internal class RestDosageProviderConfiguration : DosageProviderConfiguration
{
    internal RestConfiguration? EndPoint { get; set; }
}

