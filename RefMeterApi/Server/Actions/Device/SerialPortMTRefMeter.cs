using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
///
/// </summary>
public interface ISerialPortMTRefMeter : IRefMeter
{
}

/// <summary>
/// Handle all requests to a MT786 compatible devices.
/// </summary>
public partial class SerialPortMTRefMeter : ISerialPortMTRefMeter
{
    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortMTRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortMTRefMeter(ISerialPortConnection device, ILogger<SerialPortMTRefMeter> logger)
    {
        _device = device;
        _logger = logger;

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequest);
    }

    /// <inheritdoc/>
    public bool Available => true;
}
