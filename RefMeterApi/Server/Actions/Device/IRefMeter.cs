using RefMeterApi.Models;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Represents a reference meter device.
/// </summary>
public interface IRefMeter
{
    /// <summary>
    /// Queries a device connected to the serial port for the current
    /// measurement results.
    /// </summary>
    /// <returns>All measurement data.</returns>
    Task<MeasureOutput> GetActualValues();

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
    /// Set the DOS mode.
    /// </summary>
    /// <param name="on">set to turn on.</param>
    Task SetDosageMode(bool on);

    /// <summary>
    /// Define the dosage energy.
    /// </summary>
    /// <param name="value">Value in Wh.</param>
    Task SetDosageEnergy(double value);

    /// <summary>
    /// Start a dosage measurement.
    /// </summary>
    Task StartDosage();

    /// <summary>
    /// Terminate a dosage measurement.
    /// </summary>
    Task CancelDosage();

    /// <summary>
    /// Reports the remaining energy in the current dosage operation.
    /// </summary>
    /// <returns>Information on the current progress of the dosage measurement.</returns>
    Task<DosageProgress> GetDosageProgress();
}
