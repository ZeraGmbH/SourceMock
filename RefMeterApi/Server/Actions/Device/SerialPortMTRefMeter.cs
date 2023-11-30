using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Handle all requests to a MT786 compatible devices.
/// </summary>
public partial class SerialPortMTRefMeter : IRefMeter
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortMTRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortMTRefMeter(SerialPortConnection device, ILogger<SerialPortMTRefMeter> logger)
    {
        _device = device;
        _logger = logger;

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequest);
    }
}
