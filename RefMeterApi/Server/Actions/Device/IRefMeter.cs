using RefMeterApi.Models;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Represents a reference meter device.
/// </summary>
public interface IRefMeter
{
    /// <summary>
    /// Set if the reference meter is fully configured and can be used.
    /// </summary>
    bool Available { get; }

    /// <summary>
    /// Queries a device connected to the serial port for the current
    /// measurement results.
    /// </summary>
    /// <param name="firstActiveCurrentPhase">Index of the first active voltage phase if known.</param>
    /// <returns>All measurement data.</returns>
    Task<MeasuredLoadpoint> GetActualValues(int firstActiveCurrentPhase = -1);

    /// <summary>
    /// Read all supported measurment modes.
    /// </summary>
    /// <returns></returns>
    Task<MeasurementModes[]> GetMeasurementModes();

    /// <summary>
    /// Get the active measurement mode.
    /// </summary>
    /// <returns>The currently active measurement mode.</returns>
    Task<MeasurementModes?> GetActualMeasurementMode();

    /// <summary>
    /// Set the active measurement mode.
    /// </summary>
    /// <param name="mode">The new measurement mode.</param>
    Task SetActualMeasurementMode(MeasurementModes mode);

    /// <summary>
    /// Report the current megter constant of the reference meter (impulses per kWh).
    /// </summary>
    Task<double> GetMeterConstant();
}
