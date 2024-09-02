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
    Task<bool> GetAvailableAsync(IInterfaceLogger logger);

    /// <summary>
    /// Configure the error measurement.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="dutMeterConstant">The meter constant of the device under test - impulses per kWh.</param>
    /// <param name="impulses">Number of impluses to sent.</param>
    /// <param name="refMeterMeterConstant">The meter constant of the reference meter - impulses per kWh.</param>
    Task SetErrorMeasurementParametersAsync(IInterfaceLogger logger, MeterConstant dutMeterConstant, Impulses impulses, MeterConstant refMeterMeterConstant);

    /// <summary>
    /// Start the error measurement.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="continuous">Unset for a single measurement.</param>
    /// <param name="connection">The physical line to use.</param>
    Task StartErrorMeasurementAsync(IInterfaceLogger logger, bool continuous, ErrorCalculatorMeterConnections? connection);

    /// <summary>
    /// Report all connections that can be used as a parameter of StartErrorMeasurement.
    /// </summary>
    /// <returns>List of supported connections, may be empty.</returns>
    Task<ErrorCalculatorMeterConnections[]> GetSupportedMeterConnectionsAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Terminate the error measurement.
    /// </summary>
    Task AbortErrorMeasurementAsync(IInterfaceLogger logger);

    /// <summary>
    /// Terminate all outstanding operations.
    /// </summary>
    /// <returns></returns>
    Task AbortAllJobsAsync(IInterfaceLogger logger);

    /// <summary>
    /// Report the current status of the error measurement.
    /// </summary>
    /// <returns>The current status.</returns>
    Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger logger);

    /// <summary>
    /// Retrieve the firmware version of the error calculator.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger);

    /// <summary>
    /// Make sure that source is connected to the device under test.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="on">Set to connect else disconnect.</param>
    Task ActivateSourceAsync(IInterfaceLogger logger, bool on);

    /// <summary>
    /// Retrieve the number of impulses detected from the
    /// device under test.
    /// </summary>
    /// <param name="logger"></param>
    /// <returns>Number of impulses since last counter reset or
    /// null if readout is not supported.</returns>
    Task<Impulses?> GetNumberOfDeviceUnderTestImpulsesAsync(IInterfaceLogger logger);
}
