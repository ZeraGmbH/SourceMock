using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Handle all requests to a device.
/// </summary>
public partial class SerialPortErrorCalculatorDevice : IErrorCalculator
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortErrorCalculatorDevice> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortErrorCalculatorDevice(SerialPortConnection device, ILogger<SerialPortErrorCalculatorDevice> logger)
    {
        _device = device;
        _logger = logger;
    }
}
