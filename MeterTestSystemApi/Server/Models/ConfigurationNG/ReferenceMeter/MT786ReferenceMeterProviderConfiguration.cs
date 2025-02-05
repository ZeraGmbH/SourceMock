using SerialPortProxy;

namespace MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;

/// <summary>
/// 
/// </summary>
public class MT786ReferenceMeterProviderConfiguration : ReferenceMeterProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}
