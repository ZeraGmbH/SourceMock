using System.Text.RegularExpressions;
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
    public async Task<double> GetMeterConstant()
    {
        TestConfigured();

        /* Request raw data from device. */
        var biRequest = SerialPortRequest.Create("BI", new Regex(@"^BI(.+)$"));
        var buRequest = SerialPortRequest.Create("BU", new Regex(@"^BU(.+)$"));

        await Task.WhenAll(_device.Execute(biRequest, buRequest));

        /* Convert text representations to numbers. */
        var voltage = double.Parse(buRequest.EndMatch!.Groups[1].Value);
        var current = double.Parse(biRequest.EndMatch!.Groups[1].Value);

        var phases = SupportedModes.Single(e => e.Value == _measurementMode).Key[0] switch
        {
            '4' => 3d,
            '3' => 2d,
            '2' => 1d,
            _ => throw new ArgumentException($"unsupported measurement mode {_measurementMode}")
        };

        /* Calculate according to formula - see MT78x_MAN_EXT_GB.pdf section 5.6.*/
        return 1000d * 3600d * 60000d / (phases * (double)voltage * (double)current);
    }

    /// <summary>
    /// See if the reference meter is configured.
    /// </summary>
    /// <exception cref="RefMeterNotReadyException">Reference meter is not configured properly.</exception>
    private void TestConfigured()
    {
        if (!Available) throw new RefMeterNotReadyException();
    }
}
