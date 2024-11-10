using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Source;

/// <summary>
/// 
/// </summary>
public class FG30xSourceProviderConfiguration : SourceProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}
