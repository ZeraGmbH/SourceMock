using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Interface to configure an error calculator.
/// </summary>
public interface ISerialPortMTErrorCalculator : IErrorCalculator
{
}

/// <summary>
/// Handle all requests to a MT compatible device.
/// </summary>
/// <remarks>
/// Initialize device manager.
/// </remarks>
/// <param name="device">Service to access the current serial port.</param>
/// <param name="logger">Logging service for this device type.</param>
public partial class SerialPortMTErrorCalculator(ISerialPortConnection device, ILogger<SerialPortMTErrorCalculator> logger) : ISerialPortMTErrorCalculator
{
    /// <summary>
    /// Communication interface to the device.
    /// </summary>
    private readonly ISerialPortConnection _device = device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortMTErrorCalculator> _logger = logger;

    /// <inheritdoc/>
    public bool Available => true;
}
