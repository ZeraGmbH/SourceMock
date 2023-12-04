using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface ISerialPortMTErrorCalculator : IErrorCalculator
{
}

/// <summary>
/// Handle all requests to a MT compatible device.
/// </summary>
public partial class SerialPortMTErrorCalculator : ISerialPortMTErrorCalculator
{
    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortMTErrorCalculator> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortMTErrorCalculator(ISerialPortConnection device, ILogger<SerialPortMTErrorCalculator> logger)
    {
        _device = device;
        _logger = logger;
    }
}
