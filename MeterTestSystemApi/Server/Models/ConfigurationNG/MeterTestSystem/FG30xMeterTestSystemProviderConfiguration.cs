using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;

/// <summary>
/// 
/// </summary>
public class FG30xMeterTestSystemProviderConfiguration : MeterTestSystemProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public AmplifiersAndReferenceMeterConfiguration? AmplifiersAndReferenceMeter { get; set; }
}
