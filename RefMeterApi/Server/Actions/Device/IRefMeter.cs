using RefMeterApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace RefMeterApi.Actions.Device;

/// <summary>
/// Represents a reference meter device.
/// </summary>
public interface IRefMeter
{
    /// <summary>
    /// Set if the reference meter is fully configured and can be used.
    /// </summary>
    Task<bool> GetAvailableAsync(IInterfaceLogger logger);

    /// <summary>
    /// Queries a device connected to the serial port for the current
    /// measurement results.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="firstActiveCurrentPhase">Index of the first active voltage phase if known.</param>
    /// <returns>All measurement data.</returns>
    Task<MeasuredLoadpoint> GetActualValuesAsync(IInterfaceLogger logger, int firstActiveCurrentPhase = -1);

    /// <summary>
    /// Read all supported measurment modes.
    /// </summary>
    /// <returns></returns>
    Task<MeasurementModes[]> GetMeasurementModesAsync(IInterfaceLogger logger);

    /// <summary>
    /// Get the active measurement mode.
    /// </summary>
    /// <returns>The currently active measurement mode.</returns>
    Task<MeasurementModes?> GetActualMeasurementModeAsync(IInterfaceLogger logger);

    /// <summary>
    /// Set the active measurement mode.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mode">The new measurement mode.</param>
    Task SetActualMeasurementModeAsync(IInterfaceLogger logger, MeasurementModes mode);

    /// <summary>
    /// Report the current megter constant of the reference meter (impulses per kWh).
    /// </summary>
    Task<MeterConstant> GetMeterConstantAsync(IInterfaceLogger logger);

    /// <summary>
    /// Retrieve information on the reference meter.
    /// </summary>
    Task<ReferenceMeterInformation> GetMeterInformationAsync(IInterfaceLogger logger);
    

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Voltage[]> GetVoltageRangesAsync();
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Current[]> GetCurrentRangesAsync();

        /// <summary>
        /// Set voltage range - automatic should be disabled
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public Task SetVoltageRangeAsync(Voltage voltage);

        /// <summary>
        /// Set current range - automatic should be disabled
        /// </summary>
        /// <param name="current">Upper value of selected range</param>
        /// <returns></returns>
        public Task SetCurrentRangeAsync(Current current);

        /// <summary>
        /// Set wheter the ranges should be set automatic
        /// </summary>
        /// <param name="voltageRanges"></param>
        /// <param name="currentRanges"></param>
        /// <param name="pll"></param>
        /// <returns></returns>
        public Task SetAutomaticAsync(bool voltageRanges=true, bool currentRanges=true, bool pll=true);

        /// <summary>
        /// Select PLL chanel - automatic should be disabled
        /// </summary>
        /// <param name="pll"></param>
        /// <returns></returns>
        public Task SelectPllChannelAsync(PllChannel pll);
}
