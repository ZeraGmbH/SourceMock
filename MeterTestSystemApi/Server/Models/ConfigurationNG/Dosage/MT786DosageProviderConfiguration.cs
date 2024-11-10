using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

internal class MT786DosageProviderConfiguration : DosageProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}

