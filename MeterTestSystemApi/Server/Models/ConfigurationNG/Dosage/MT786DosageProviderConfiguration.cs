using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

/// <summary>
/// 
/// </summary>
public class MT786DosageProviderConfiguration : DosageProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}

