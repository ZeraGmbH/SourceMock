using SourceApi.Model.Configuration;

namespace BurdenApi.Models;

/// <summary>
/// 
/// </summary>
public class BurdenConfiguration
{
    /// <summary>
    /// Serial port to connect to the burden (ESCB or ESVB).
    /// </summary>
    public SerialPortConfiguration? SerialPort { get; set; }
}