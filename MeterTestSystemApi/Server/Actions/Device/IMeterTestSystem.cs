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
    Task SetAmplifiersAndReferenceMeter(AmplifiersAndReferenceMeter settings);

    /// <summary>
    /// Retrive the current error condition overview for the meter test system.
    /// </summary>
    /// <returns>All available error conditions.</returns>
    Task<ErrorConditions> GetErrorConditions();

    /// <summary>
    /// Request the current physical configuration used.
    /// </summary>
    AmplifiersAndReferenceMeter AmplifiersAndReferenceMeter { get; }

    /// <summary>
    /// The corresponding source.
    /// </summary>
    ISource Source { get; }

    /// <summary>
    /// The related reference meter.
    /// </summary>
    IRefMeter RefMeter { get; }

    /// <summary>
    /// The error calculators associated with this metering system - must be at least
    /// one.
    /// </summary>
    IErrorCalculator[] ErrorCalculators { get; }

    /// <summary>
    /// Fired whenever a new error condition is detected.
    /// </summary>
    event Action<ErrorConditions> ErrorConditionsChanged;
}
