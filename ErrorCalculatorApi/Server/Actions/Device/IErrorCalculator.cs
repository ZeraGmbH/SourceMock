using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// API for any error calculator device.
/// </summary>
public interface IErrorCalculator
{
    /// <summary>
    /// Set if the error calculator is fully configured and can be used.
    /// </summary>
    bool GetAvailable(IInterfaceLogger logger);

    /// <summary>
    /// Configure the error measurement.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dutMeterConstant">The meter constant of the device under test - impulses per kWh.</param>
    /// <param name="impulses">Number of impluses to sent.</param>
    /// <param name="refMeterMeterConstant">The meter constant of the reference meter - impulses per kWh.</param>
    Task SetErrorMeasurementParameters(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant);

    /// <summary>
    /// Start the error measurement.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="continuous">Unset for a single measurement.</param>
    /// <param name="connection">The physical line to use.</param>
    Task StartErrorMeasurement(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection);

    /// <summary>
    /// Report all connections that can be used as a parameter of StartErrorMeasurement.
    /// </summary>
    /// <returns>List of supported connections, may be empty.</returns>
    Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnections(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Terminate the error measurement.
    /// </summary>
    Task AbortErrorMeasurement(IInterfaceLogger logger);

    /// <summary>
    /// Terminate all outstanding operations.
    /// </summary>
    /// <returns></returns>
    Task AbortAllJobs(IInterfaceLogger logger);

    /// <summary>
    /// Report the current status of the error measurement.
    /// </summary>
    /// <returns>The current status.</returns>
    Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger);

    /// <summary>
    /// Retrieve the firmware version of the error calculator.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger);

    /// <summary>
    /// Make sure that source is connected to the device under test.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="on">Set to connect else disconnect.</param>
    Task ActivateSource(IInterfaceLogger logger, bool on);
}
