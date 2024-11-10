using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

internal class MT786ReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}
