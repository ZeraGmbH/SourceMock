using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;

internal class FG30xMeterTestSystemProviderConfiguration : MeterTestSystemProviderConfiguration
{
    internal SerialPortConfiguration? SerialPort { get; set; }

    internal AmplifiersAndReferenceMeterConfiguration? AmplifiersAndReferenceMeter { get; set; }
}
