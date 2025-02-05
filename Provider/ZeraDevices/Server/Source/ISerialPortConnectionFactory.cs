using SerialPortProxy;
using ZERA.WebSam.Shared.Models;

namespace ZeraDevices.Source;

/// <summary>
/// 
/// </summary>
public interface ISerialPortConnectionFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="configuration"></param>
    public void Initialize(MeterTestSystemTypes? type, SerialPortConfiguration? configuration);

    /// <summary>
    /// 
    /// </summary>
    public ISerialPortConnection Connection { get; }
}
