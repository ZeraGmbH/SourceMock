using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging;
using SerialPortProxy;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Configuration interface for an error calculator connected
/// to a frequency configurator.
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
/// <remarks>
/// Initialize device manager.
/// </remarks>
/// <param name="device">Service to access the current serial port.</param>
/// <param name="logger">Logging service for this device type.</param>

public class SerialPortFGErrorCalculator(ISerialPortConnection device, ILogger<SerialPortFGErrorCalculator> logger) : ISerialPortFGErrorCalculator
{
    /// <summary>
    /// Serial port connection.
    /// </summary>
    private readonly ISerialPortConnection _device = device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortFGErrorCalculator> _logger = logger;

    /// <inheritdoc/>
    public bool Available => false;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement() => throw new NotImplementedException();

    /// <inheritdoc/>

    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(double dutMeterConstant, long impulses, double refMeterMeterConstant) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task StartErrorMeasurement(bool continuous) => throw new NotImplementedException();
}
