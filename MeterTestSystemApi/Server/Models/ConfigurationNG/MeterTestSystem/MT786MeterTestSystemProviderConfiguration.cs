using SerialPortProxy;

namespace MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;

/// <summary>
/// 
/// </summary>
public class MT786MeterTestSystemProviderConfiguration : MeterTestSystemProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}
