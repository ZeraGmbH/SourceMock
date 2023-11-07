using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Handle all requests to a device.
/// </summary>
public partial class SerialPortRefMeterDevice : IRefMeterDevice
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortRefMeterDevice> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortRefMeterDevice(SerialPortConnection device, ILogger<SerialPortRefMeterDevice> logger)
    {
        _device = device;
        _logger = logger;

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequest);
    }

}
