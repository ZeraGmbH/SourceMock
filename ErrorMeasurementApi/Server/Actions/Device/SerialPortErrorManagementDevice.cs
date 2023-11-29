using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorMeasurementApi.Actions.Device;

/// <summary>
/// Handle all requests to a device.
/// </summary>
public partial class SerialPortErrorManagementDevice : IErrorMeasurement
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortErrorManagementDevice> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortErrorManagementDevice(SerialPortConnection device, ILogger<SerialPortErrorManagementDevice> logger)
    {
        _device = device;
        _logger = logger;
    }
}
