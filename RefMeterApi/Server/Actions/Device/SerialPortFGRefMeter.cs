using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using RefMeterApi.Actions.MeterConstantCalculator;
using RefMeterApi.Exceptions;
using RefMeterApi.Models;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Configuration interface for a reference meter connected to
/// a frequency generator.
/// </summary>
public interface ISerialPortFGRefMeter : IRefMeter
{
    /// <summary>
    /// Reference meter to use.
    /// </summary>
    /// <param name="refereneceMeter">Reference meter.</param>
    void SetReferenceMeter(ReferenceMeters refereneceMeter);
}

/// <summary>
/// Handle all requests to a FG30x compatible devices.
/// </summary>
public partial class SerialPortFGRefMeter : ISerialPortFGRefMeter
{
    /// <summary>
    /// Initialize device manager.
    /// </summary>
    /// <param name="device">Service to access the current serial port.</param>
    /// <param name="meterConstant">Algorithm to calculate the meter constant.</param>
    public SerialPortFGRefMeter([FromKeyedServices("MeterTestSystem")] ISerialPortConnection device, IMeterConstantCalculator meterConstant)
    {
        _device = device.CreateExecutor(InterfaceLogSourceTypes.ReferenceMeter);
        _meterConstant = meterConstant;

        /* Setup caches for shared request results. */
        _actualValues = new(CreateActualValueRequestAsync, 1000);
    }

    /// <summary>
    /// Device connection to use.
    /// </summary>
    private readonly ISerialPortConnectionExecutor _device;

    private readonly IMeterConstantCalculator _meterConstant;

    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_referenceMeter.HasValue);

    /// <summary>
    /// The reference meter to use.
    /// </summary>
    private ReferenceMeters? _referenceMeter;

    /// <inheritdoc/>
    public async Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger)
    {
        await TestConfiguredAsync(logger);

        /* Request raw data from device. */
        var biRequest = SerialPortRequest.Create("BI", new Regex(@"^BI(.+)$"));
        var buRequest = SerialPortRequest.Create("BU", new Regex(@"^BU(.+)$"));
        var fiRequest = SerialPortRequest.Create("FI", new Regex(@"^FI(.+)$"));

        await Task.WhenAll(_device.ExecuteAsync(logger, biRequest, buRequest, fiRequest));

        /* Convert text representations to numbers. */
        var voltage = new Voltage(double.Parse(buRequest.EndMatch!.Groups[1].Value));
        var current = new Current(double.Parse(biRequest.EndMatch!.Groups[1].Value));
        var frequency = new Frequency(double.Parse(fiRequest.EndMatch!.Groups[1].Value));

        return _meterConstant.GetMeterConstant(_referenceMeter!.Value, frequency, _measurementMode ?? MeasurementModes.FourWireActivePower, voltage, current);
    }

    /// <inheritdoc/>
    public void SetReferenceMeter(ReferenceMeters refereneceMeter)
    {
        if (_referenceMeter != null) throw new InvalidOperationException("already initialized");

        _referenceMeter = refereneceMeter;
    }

    /// <summary>
    /// See if the reference meter is configured.
    /// </summary>
    /// <exception cref="RefMeterNotReadyException">Reference meter is not configured properly.</exception>
    private async Task TestConfiguredAsync(IInterfaceLogger interfaceLogger)
    {
        if (!await GetAvailableAsync(interfaceLogger)) throw new RefMeterNotReadyException();
    }

    /// <inheritdoc/>
    public async Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger)
    {
        /* Send command and check reply. */
        var request = SerialPortRequest.Create("TS", new Regex("^TS(.{8})(.{4})$"));

        await _device.ExecuteAsync(logger, request)[0];

        return new()
        {
            Model = request.EndMatch!.Groups[1].Value.Trim(),
            NumberOfPhases = 3,
            SoftwareVersion = request.EndMatch!.Groups[2].Value.Trim()
        };
    }
}
