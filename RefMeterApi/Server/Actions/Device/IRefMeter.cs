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
    /// <param name="firstActiveVoltagePhase">Index of the first active voltage phase if known.</param>
    /// <returns>All measurement data.</returns>
    Task<MeasureOutput> GetActualValues(int firstActiveVoltagePhase = -1);

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
}
