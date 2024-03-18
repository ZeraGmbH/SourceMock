using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// API for any error calculator device.
/// </summary>
public interface IErrorCalculator
{
    /// <summary>
    /// Set if the error calculator is fully configured and can be used.
    /// </summary>
    bool Available { get; }

    /// <summary>
    /// Configure the error measurement.
    /// </summary>
    /// <param name="dutMeterConstant">The meter constant of the device under test - impulses per kWh.</param>
    /// <param name="impulses">Number of impluses to sent.</param>
    /// <param name="refMeterMeterConstant">The meter constant of the reference meter - impulses per kWh.</param>
    Task SetErrorMeasurementParameters(double dutMeterConstant, long impulses, double refMeterMeterConstant);

    /// <summary>
    /// Start the error measurement.
    /// </summary>
    /// <param name="continuous">Unset for a single measurement.</param>
    /// <param name="connection">The physical line to use.</param>
    Task StartErrorMeasurement(bool continuous, ErrorCalculatorConnections? connection);

    /// <summary>
    /// Report all connections that can be used as a parameter of StartErrorMeasurement.
    /// </summary>
    /// <returns>List of supported connections, may be empty.</returns>
    Task<ErrorCalculatorConnections[]> GetSupportedConnections();

    /// <summary>
    /// Terminate the error measurement.
    /// </summary>
    Task AbortErrorMeasurement();

    /// <summary>
    /// Terminate all outstanding operations.
    /// </summary>
    /// <returns></returns>
    Task AbortAllJobs();

    /// <summary>
    /// Report the current status of the error measurement.
    /// </summary>
    /// <returns>The current status.</returns>
    Task<ErrorMeasurementStatus> GetErrorStatus();

    /// <summary>
    /// Retrieve the firmware version of the error calculator.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion();

    /// <summary>
    /// Make sure that source is connected to the device under test.
    /// </summary>
    /// <param name="on">Set to connect else disconnect.</param>
    Task ActivateSource(bool on);
}
