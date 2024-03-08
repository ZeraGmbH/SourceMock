using Microsoft.Extensions.Logging;
using RefMeterApi.Exceptions;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Configuration interface for a reference meter connected to
/// a frequency generator.
/// </summary>
public interface ISerialPortFGRefMeter : IRefMeter
{
}

/// <summary>
/// Handle all requests to a FG30x compatible devices.
/// </summary>
/// <remarks>
/// Initialize device manager.
/// </remarks>
/// <param name="device">Service to access the current serial port.</param>
/// <param name="logger">Logging service for this device type.</param>
public partial class SerialPortFGRefMeter(ISerialPortConnection device, ILogger<SerialPortFGRefMeter> logger) : ISerialPortFGRefMeter
{
    /// <summary>
    /// Device connection to use.
    /// </summary>
    private readonly ISerialPortConnection _device = device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortFGRefMeter> _logger = logger;

    /// <inheritdoc/>
    public bool Available => true;

    /// <inheritdoc/>
    public Task<double> GetMeterConstant() => throw new NotImplementedException();

    /// <summary>
    /// See if the reference meter is configured.
    /// </summary>
    /// <exception cref="RefMeterNotReadyException">Reference meter is not configured properly.</exception>
    private void TestConfigured()
    {
        if (!Available) throw new RefMeterNotReadyException();
    }
}
