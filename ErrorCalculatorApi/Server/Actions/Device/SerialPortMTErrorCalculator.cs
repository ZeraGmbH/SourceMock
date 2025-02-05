using System.Text.RegularExpressions;
using ZERA.WebSam.Shared.Models.ErrorCalculator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// Interface to configure an error calculator.
/// </summary>
public interface ISerialPortMTErrorCalculator : IErrorCalculator
{
}

/// <summary>
/// Handle all requests to a MT compatible device.
/// </summary>
/// <remarks>
/// Initialize device manager.
/// </remarks>
/// <param name="device">Service to access the current serial port.</param>
/// <param name="logger">Logging service for this device type.</param>
public partial class SerialPortMTErrorCalculator([FromKeyedServices("MeterTestSystem")] ISerialPortConnection device, ILogger<SerialPortMTErrorCalculator> logger) : ISerialPortMTErrorCalculator
{
    /// <summary>
    /// Detect model name and version number.
    /// </summary>
    private static readonly Regex _versionReg = new("^(.+)V([^V]+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>
    /// Communication interface to the device.
    /// </summary>
    private readonly ISerialPortConnectionExecutor _device = device.CreateExecutor(InterfaceLogSourceTypes.ErrorCalculator, "0");

    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortMTErrorCalculator> _logger = logger;

    /// <inheritdoc/>
    public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

    /// <inheritdoc/>
    public Task AbortAllJobsAsync(IInterfaceLogger logger) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task ActivateSourceAsync(IInterfaceLogger logger, bool on) => Task.CompletedTask;

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public async Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger)
    {
        /* Execute the request and wait for the information string. */
        var reply = await _device.ExecuteAsync(logger, SerialPortRequest.Create("AAV", "AAVACK"))[0];

        if (reply.Length < 2)
            throw new InvalidOperationException($"wrong number of response lines - expected 2 but got {reply.Length}");

        /* Validate the response consisting of model name and version numner. */
        var versionMatch = _versionReg.Match(reply[^2]);

        if (versionMatch?.Success != true)
            throw new InvalidOperationException($"invalid response {reply[0]} from device");

        /* Create response structure. */
        return new ErrorCalculatorFirmwareVersion
        {
            ModelName = versionMatch.Groups[1].Value,
            Version = versionMatch.Groups[2].Value
        };
    }
}
