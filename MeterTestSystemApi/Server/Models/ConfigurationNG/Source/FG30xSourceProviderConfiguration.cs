using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

internal class FG30xSourceProviderConfiguration : SourceProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}
