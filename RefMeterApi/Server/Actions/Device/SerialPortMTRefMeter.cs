using Microsoft.Extensions.Logging;
using SerialPortProxy;

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
    private readonly ISerialPortConnection _device;

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortMTRefMeter> _logger;

    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="logger">Logging service for this device type.</param>
    public SerialPortMTRefMeter(ISerialPortConnection device, ILogger<SerialPortMTRefMeter> logger)
    {
        _device = device;
        _logger = logger;

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequest);
    }

    /// <inheritdoc/>
    public bool Available => true;

    /// <inheritdoc/>
    public async Task<double> GetMeterConstant()
    {
        var reply = await _device.Execute(SerialPortRequest.Create("AST", "ASTACK"))[0];

        /* We need the range of voltage and current and the measurement mode as well. */
        double? voltage = null, current = null;
        string? mode = null;

        foreach (var value in reply)
            if (value.StartsWith("UB="))
                voltage = double.Parse(value.Substring(3));
            else if (value.StartsWith("IB="))
                current = double.Parse(value.Substring(3));
            else if (value.StartsWith("M="))
                mode = value.Substring(2);

        if (!voltage.HasValue || !current.HasValue || string.IsNullOrEmpty(mode))
            throw new InvalidOperationException("AST status incomplete");

        var phases =
            mode[0] == '4' ? 3d :
            mode[0] == '3' ? 2d :
            mode[0] == '2' ? 1d :
            throw new ArgumentException($"unsupported measurement mode {mode}");

        /* Calculate according to formula - see MT78x_MAN_EXT_GB.pdf section 5.6.*/
        return 1000d * 3600d * 60000d / (phases * (double)voltage * (double)current);
    }
}
