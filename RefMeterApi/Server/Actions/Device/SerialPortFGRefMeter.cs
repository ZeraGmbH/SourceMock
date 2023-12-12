using Microsoft.Extensions.Logging;
using RefMeterApi.Exceptions;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface ISerialPortFGRefMeter : IRefMeter
{
}

/// <summary>
/// Handle all requests to a FG30x compatible devices.
/// </summary>
public partial class SerialPortFGRefMeter : ISerialPortFGRefMeter
{
    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortFGRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortFGRefMeter(ISerialPortConnection device, ILogger<SerialPortFGRefMeter> logger)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool Available => true;

    private void TestConfigured()
    {
        if (!Available) throw new RefMeterNotReadyException();
    }
}
