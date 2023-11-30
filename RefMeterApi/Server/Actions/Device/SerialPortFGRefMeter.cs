using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Handle all requests to a FG30x compatible devices.
/// </summary>
public class SerialPortFGRefMeter : IRefMeter
{
    private readonly SerialPortConnection _device;

    private readonly ILogger<SerialPortFGRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortFGRefMeter(SerialPortConnection device, ILogger<SerialPortFGRefMeter> logger)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<MeasurementModes?> GetActualMeasurementMode()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MeasureOutput> GetActualValues()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<MeasurementModes[]> GetMeasurementModes()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Task SetActualMeasurementMode(MeasurementModes mode)
    {
        throw new NotImplementedException();
    }
}
