using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface ISerialPortFGErrorCalculator : IErrorCalculator
{
}

/// <summary>
/// Handle all requests to a FG30x compatible device. There is
/// no error calculaor available using the serial port communication,
/// it has to be connected separatly (via network and Koala XML 
/// protocol).
/// </summary>

public class SerialPortFGErrorCalculator : ISerialPortFGErrorCalculator
{
    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortFGErrorCalculator> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortFGErrorCalculator(ISerialPortConnection device, ILogger<SerialPortFGErrorCalculator> logger)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool Available => false;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double meterConstant, long impulses)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous)
    {
        throw new NotImplementedException();
    }
}
