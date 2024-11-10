using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

internal class MT786SourceProviderConfiguration : SourceProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}
