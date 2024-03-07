using SerialPortProxy;
using SharedLibrary.Models;
using SourceApi.Model.Configuration;

namespace SourceApi.Actions.SerialPort;

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
