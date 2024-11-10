using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG.Dosage;

/// <summary>
/// 
/// </summary>
public class FG30xDosageProviderConfiguration : DosageProviderConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}

