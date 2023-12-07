using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// 
/// </summary>
public interface ISerialPortFGRefMeter : IRefMeter
{
}

/// <summary>
/// Handle all requests to a FG30x compatible devices.
/// </summary>
public partial class SerialPortFGRefMeter : ISerialPortFGRefMeter
{
    private readonly ISerialPortConnection _device;

    private readonly ILogger<SerialPortFGRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortFGRefMeter(ISerialPortConnection device, ILogger<SerialPortFGRefMeter> logger)
    {
        _device = device;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool Available => true;

    /// <inheritdoc/>
    public Task<MeasureOutput> GetActualValues()
    {
        // Must be requested separatly
        //  Phase Order: unknown, may be derivable
        //  Phase
        //  - Voltage: AU/AD and BU
        //  - Current: AI and BI
        //  - Angles: AW
        //  - Power Factor: unknown, may be derivable
        //  - Active Power: MP
        //  - Reactive Power: MQ
        //  - Apparent Power: MS
        //  Active Power - must be summed up from phases
        //  Reactive Power - must be summed up from phases
        //  Apparent Power - must be summed up from phases
        //  Frequency: AF
        throw new NotImplementedException();
    }
}
