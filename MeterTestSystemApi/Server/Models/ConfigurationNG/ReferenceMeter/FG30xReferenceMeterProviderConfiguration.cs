using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

/// <summary>
/// 
/// </summary>
public class FG30xReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}
