using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Interface to configure a reference meter connected to 
/// a movable meter test system.
/// </summary>
public interface ISerialPortMTRefMeter : IRefMeter
{
}

/// <summary>
/// Handle all requests to a MT786 compatible devices.
/// </summary>
public partial class SerialPortMTRefMeter : ISerialPortMTRefMeter
{
    /// <summary>
    /// Connection to the device.
    /// </summary>
    private readonly ISerialPortConnectionExecutor _device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortMTRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortMTRefMeter([FromKeyedServices("MeterTestSystem")] ISerialPortConnection device, ILogger<SerialPortMTRefMeter> logger)
    {
        _logger = logger;

        _device = device.CreateExecutor(InterfaceLogSourceTypes.ReferenceMeter);

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequest, 1000);
    }

    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => true;

    /// <inheritdoc/>
    public async Task<MeterConstant> GetMeterConstant(IInterfaceLogger logger)
    {
        var reply = await _device.Execute(logger, SerialPortRequest.Create("AST", "ASTACK"))[0];

        /* We need the range of voltage and current and the measurement mode as well. */
        double? voltage = null, current = null;
        string? mode = null;

        foreach (var value in reply)
            if (value.StartsWith("UB="))
                voltage = double.Parse(value[3..]);
            else if (value.StartsWith("IB="))
                current = double.Parse(value[3..]);
            else if (value.StartsWith("M="))
                mode = value[2..];

        if (!voltage.HasValue || !current.HasValue || string.IsNullOrEmpty(mode))
            throw new InvalidOperationException("AST status incomplete");

        var phases =
            mode[0] == '4' ? 3d :
            mode[0] == '3' ? 2d :
            mode[0] == '2' ? 1d :
            throw new ArgumentException($"unsupported measurement mode {mode}");

        /* Calculate according to formula - see MT78x_MAN_EXT_GB.pdf section 5.6.*/
        return new(1000d * 3600d * 60000d / (phases * voltage.Value * current.Value));
    }
}
