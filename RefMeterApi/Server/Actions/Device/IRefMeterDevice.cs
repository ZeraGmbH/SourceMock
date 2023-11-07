using RefMeterApi.Models;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Represents a reference meter device.
/// </summary>
public interface IRefMeterDevice
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
}
