using SerialPortProxy;

namespace BurdenApi.Models;

/// <summary>
/// 
/// </summary>
public interface IBurdenFactory : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    void Initialize(BurdenConfiguration configuration);

    /// <summary>
    /// 
    /// </summary>
    ISerialPortConnection Connection { get; }

    /// <summary>
    /// Create a hardware accessor.
    /// </summary>
    /// <returns>Hardware accessor according to configuration.</returns>
    ICalibrationHardware CreateHardware(IServiceProvider services);
}