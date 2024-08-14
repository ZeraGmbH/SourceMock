using ErrorCalculatorApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SerialPortProxy;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

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
/// <param name="logger">Logging service for this device type.</param>

public class SerialPortFGErrorCalculator(ILogger<SerialPortFGErrorCalculator> logger) : ISerialPortFGErrorCalculator
{
    /// <summary>
    /// Logging helper.
    /// </summary>
    private readonly ILogger<SerialPortFGErrorCalculator> _logger = logger;

    /// <inheritdoc/>
    public bool GetAvailable(IInterfaceLogger interfaceLogger) => false;

    /// <inheritdoc/>
    public Task AbortErrorMeasurement(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>

    public void Dispose()
    {
    }

    /// <inheritdoc/>
    public Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task SetErrorMeasurementParameters(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task AbortAllJobs(IInterfaceLogger logger) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task ActivateSource(IInterfaceLogger logger, bool on) => Task.CompletedTask;
}
