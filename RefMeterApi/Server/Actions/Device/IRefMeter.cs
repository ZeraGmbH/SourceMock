using RefMeterApi.Models;
using SharedLibrary.DomainSpecific;
using SharedLibrary.Models.Logging;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Represents a reference meter device.
/// </summary>
public interface IRefMeter
{
    /// <summary>
    /// Set if the reference meter is fully configured and can be used.
    /// </summary>
    bool GetAvailable(IInterfaceLogger logger);

    /// <summary>
    /// Queries a device connected to the serial port for the current
    /// measurement results.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="firstActiveCurrentPhase">Index of the first active voltage phase if known.</param>
    /// <returns>All measurement data.</returns>
    Task<MeasuredLoadpoint> GetActualValues(IInterfaceLogger logger, int firstActiveCurrentPhase = -1);

    /// <summary>
    /// Read all supported measurment modes.
    /// </summary>
    /// <returns></returns>
    Task<MeasurementModes[]> GetMeasurementModes(IInterfaceLogger logger);

    /// <summary>
    /// Get the active measurement mode.
    /// </summary>
    /// <returns>The currently active measurement mode.</returns>
    Task<MeasurementModes?> GetActualMeasurementMode(IInterfaceLogger logger);

    /// <summary>
    /// Set the active measurement mode.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mode">The new measurement mode.</param>
    Task SetActualMeasurementMode(IInterfaceLogger logger, MeasurementModes mode);

    /// <summary>
    /// Report the current megter constant of the reference meter (impulses per kWh).
    /// </summary>
    Task<SharedLibrary.DomainSpecific.MeterConstant> GetMeterConstant(IInterfaceLogger logger);
}
