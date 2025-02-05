using SerialPortProxy;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

/// <summary>
/// 
/// </summary>
public class MT786SourceProviderConfiguration : SourceProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}
