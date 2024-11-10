using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

internal class FG30xReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }
}
