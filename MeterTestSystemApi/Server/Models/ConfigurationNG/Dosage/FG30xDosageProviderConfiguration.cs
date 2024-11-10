using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

internal class FG30xDosageProviderConfiguration : DosageProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}

