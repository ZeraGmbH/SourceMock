using ErrorCalculatorApi.Actions.Device;
using MeterTestSystemApi.Models;
using RefMeterApi.Actions.Device;
using SourceApi.Actions.Source;

namespace MeterTestSystemApi.Actions.Device;

/// <summary>
/// Represents a meter test system - may be mobile or stationary.
/// </summary>
public interface IMeterTestSystem
{
    /// <summary>
    /// Retrieve the firmware version of the meter test system.
    /// </summary>
    /// <returns>The firmware version.</returns>
    Task<MeterTestSystemFirmwareVersion> GetFirmwareVersion();

    /// <summary>
    /// Request the capabilities of the meter test system.
    /// </summary>
    /// <returns>Capabilities if applicable.</returns>
    Task<MeterTestSystemCapabilities> GetCapabilities();

    /// <summary>
    /// Report the physical configuration to the meter test system implementation.
    /// </summary>
    /// <param name="settings">Physical configuration to use.</param>
    Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeters settings);

    /// <summary>
    /// Request the current physical configuration used.
    /// </summary>
    AmplifiersAndReferenceMeters AmplifiersAndReferenceMeters { get; }

    /// <summary>
    /// The corresponding source.
    /// </summary>
    ISource Source { get; }

    /// <summary>
    /// The related reference meter.
    /// </summary>
    IRefMeter RefMeter { get; }

    /// <summary>
    /// The error calculator associated with this metering system.
    /// </summary>
    IErrorCalculator ErrorCalculator { get; }
}
